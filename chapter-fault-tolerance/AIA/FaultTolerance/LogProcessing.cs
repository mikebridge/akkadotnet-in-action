using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AIA.FaultTolerance.FileWatcherProtocol;
using AIA.FaultTolerance.LogProcessingProtocol;

using Akka.Actor;

namespace AIA.FaultTolerance
{

    namespace DbStrategy1
    {
        public class LogProcessingApp
        {

            public static void Initialize()
            {
                List<string>  sources = new List<string> { "file:///source1/", "file:///source2/" };
                ActorSystem system = ActorSystem.Create("logprocessing");
                DbCon con = new DbCon("http://mydatabase");
                Props writerProps = Props.Create(() => new DbWriter(con));
                Props dbSuperProps = Props.Create(() => new DbSupervisor(writerProps));
                Props logProcSuperProps = Props.Create(() => new LogProcSupervisor(dbSuperProps));
                Props topLevelProps = Props.Create(() => new FileWatchingSupervisor(sources, logProcSuperProps));
                system.ActorOf(topLevelProps);
            }

            private static void Main(string[] args)
            {
                Initialize();
                Console.WriteLine("Hit a key to exit");
                Console.ReadKey();
            }


        }

        public class FileWatchingSupervisor : ReceiveActor
        {
            private IList<IActorRef> _fileWatchers;

            public FileWatchingSupervisor(IList<String> sources, Props logProcSuperProps)
            {
                _fileWatchers = sources.Select(source =>
                {
                    var logProcSupervisor = Context.ActorOf(logProcSuperProps);
                    var fileWatcher = Context.ActorOf(Props.Create(() => new FileWatcher(source, logProcSupervisor)));
                    Context.Watch(fileWatcher);
                    return fileWatcher;
                }).ToList();

                Initialize();
            }

            protected override SupervisorStrategy SupervisorStrategy()
            {
                return new AllForOneStrategy(
                    x =>
                    {
                        if (x is DiskError)
                        {
                            return Directive.Stop;
                        }
                        else return Directive.Restart; // TODO: what should the default here be?
                    });
            }

            private void Initialize()
            {
                Receive<Terminated>(message =>
                {
                    _fileWatchers = _fileWatchers.Where(w => w.Equals(message.ActorRef)).ToList();
                    if (!_fileWatchers.Any())
                    {
                        Self.Tell(PoisonPill.Instance);
                    }
                });
            }
        }

        public class FileWatcher : ReceiveActor, IFileWatchingAbilities
        {
            private readonly string _sourceUri;
            private readonly IActorRef _logProcSupervisor;

            public FileWatcher(String sourceUri, IActorRef logProcSupervisor)
            {
                _sourceUri = sourceUri;
                _logProcSupervisor = logProcSupervisor;
                Register(sourceUri);
                Initialize();
            }

            public void Register(String sourceUri)
            {
                //...
            }

            private void Initialize()
            {
                Receive<NewFile>(message =>
                    _logProcSupervisor.Tell(new LogFile(message.File)));

                Receive<SourceAbandoned>(message =>
                {
                    if (message.Uri == _sourceUri)
                        Self.Tell(PoisonPill.Instance);
                });
            }
        }

        public class LogProcSupervisor : ReceiveActor
        {
            private readonly Props _dbSupervisorProps;
            private IActorRef _dbSupervisor;
            private Props _logProcProps;
            private IActorRef _logProcessor;


            public LogProcSupervisor(Props dbSupervisorProps)
            {
                _dbSupervisorProps = dbSupervisorProps;
                _dbSupervisor = Context.ActorOf(dbSupervisorProps);
                _logProcProps = Props.Create(() => new LogProcessor(_dbSupervisor));
                _logProcessor = Context.ActorOf(_logProcProps);

                ReceiveAny(m => _logProcessor.Forward(m));

            }

            protected override SupervisorStrategy SupervisorStrategy()
            {
                return new OneForOneStrategy(
                    x =>
                    {
                        if (x is CorruptedFileException)
                        {
                            return Directive.Resume;
                        }
                        else return Directive.Restart; // TODO: what should the default here be?
                    });

            }
        }


        public class LogProcessor : ReceiveActor, ILogParsing
        {
            private readonly IActorRef _dbSupervisor;

            public LogProcessor(IActorRef dbSupervisor)
            {
                _dbSupervisor = dbSupervisor;
                Initialize();
            }

            private void Initialize()
            {
                Receive<LogFile>(message =>
                {
                    var lines = Parse(message.File);
                    foreach (var line in lines)
                    {
                        _dbSupervisor.Tell(line);
                    }
                });
            }

            public IEnumerable<Line> Parse(FileInfo file)
            {
                return null;
            }
        }

        public class DbImpatientSupervisor : ReceiveActor
        {
            private readonly IActorRef _writer;

            public DbImpatientSupervisor(Props writerProps)
            {
                _writer = Context.ActorOf(writerProps);
                ReceiveAny(m => _writer.Forward(m));
            }

            protected override SupervisorStrategy SupervisorStrategy()
            {
                return new OneForOneStrategy(
                    maxNrOfRetries: 5,
                    withinTimeMilliseconds: 60000,
                    localOnlyDecider: x =>
                    {
                        if (x is DbBrokenConnectionException)
                        {
                            return Directive.Restart;
                        }
                        else return Directive.Restart; // TODO: what should the default here be?
                    });
            }
        }

        public class DbSupervisor : ReceiveActor
        {
            public DbSupervisor(Props writerProps)
            {
                var writer = Context.ActorOf(writerProps);
                ReceiveAny(m => writer.Forward(m));
            }

            protected override SupervisorStrategy SupervisorStrategy()
            {
                return new OneForOneStrategy(
                    x =>
                    {
                        if (x is DbBrokenConnectionException)
                        {
                            return Directive.Restart;
                        }
                        else return Directive.Restart; // TODO: what should the default here be?
                    });
            }

        }

        public class DbWriter : ReceiveActor
        {
            public DbWriter(DbCon connection)
            {
                Receive<Line>(message => connection.Write(
                    new Dictionary<string, object>
                    {
                        {"time", message.Time},
                        {"message", message.Message},
                        {"message", message.MessageType}
                    }));
            }
        }
    }

    public class DbCon
    {
        public DbCon(String url)
        {
            
        }

        public void Write(IDictionary<String, Object> map)
        {
            // ...
        }

    }

    public class DiskError : Exception // TODO is this an exception?
    {
        public string Msg { get; set; }

        public DiskError(String msg)
        {
            Msg = msg;
        }
    }

    public class CorruptedFileException : Exception
    {
        public string Msg { get; private set; }
        public FileInfo File { get; private set; }

        public CorruptedFileException(String msg, FileInfo file)
        {
            Msg = msg;
            File = file;
        }
    }

    public class DbBrokenConnectionException : Exception
    {
        public String Msg { get; private set; }

        public DbBrokenConnectionException(String msg)
        {
            Msg = msg;
        }
    }

    public interface ILogParsing
    {
        IEnumerable<Line> Parse(FileInfo file);
    }

    namespace FileWatcherProtocol {
        public class NewFile
        {
            public FileInfo File { get; private set; }
            public long TimeAdded { get; private set; }

            public NewFile(FileInfo file, long timeAdded)
            {
                File = file;
                TimeAdded = timeAdded;
            }
        }

        public class SourceAbandoned
        {
            public string Uri { get; private set; }

            public SourceAbandoned(String uri)
            {
                Uri = uri;
            }
        }

        public interface IFileWatchingAbilities
        {
            void Register(String uri);
        }

    }


    namespace LogProcessingProtocol
    {
        public class LogFile
        {
            public FileInfo File { get; private set; }

            public LogFile(FileInfo file)
            {
                File = file;
            }
        }

        public class Line
        {
            public long Time { get; private set; }
            public string Message { get; private set; }
            public string MessageType { get; private set; }

            public Line(long time, String message, String messageType)
            {
                Time = time;
                Message = message;
                MessageType = messageType;
            }
        }
    }

}
