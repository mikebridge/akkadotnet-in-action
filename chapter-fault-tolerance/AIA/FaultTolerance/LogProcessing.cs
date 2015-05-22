using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIA.FaultTolerance
{
    

    public class LogProcessing
    {
    }

    namespace LogProcessingControl
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
