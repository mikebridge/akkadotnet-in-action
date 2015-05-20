## Akka in Action for Akka.Net

Example

```bash
$ curl http://localhost:50494/events
--> []

$  curl -X POST http://localhost:50494/events -d 'name=RHCP&tickets=10'
--> {}

$ curl http://localhost:50494/events
--> [{"Name":"RHCP","Tickets":10}]

$ curl -I http://localhost:50494/events/UnknownEvent
--> HTTP/1.1 404 Not Found ...

$ curl -X POST  http://localhost:50494/events -d 'name=RHCP&tickets=10'
--> {"Message":"RHCP event exists already."}

$ curl http://localhost:50494/events/RHCP
--> {"Name":"RHCP","Tickets":10}

$ curl -X POST http://localhost:50494/events/RHCP/tickets -d "tickets=2"
--> {"Event":"RHCP","Entries":[{"Id":1},{"Id":2}]}

$ curl http://localhost:50494/events/RHCP
--> {"Name":"RHCP","Tickets":8}

$ curl -X POST http://localhost:50494/events/RHCP/tickets -d "tickets=9"
--> HTTP/1.1 404 Not Found ...

$ curl -I -X DELETE http://localhost:50494/events/RHCP
--> HTTP/1.1 200 OK ...

$ curl http://localhost:50494/events
--> []