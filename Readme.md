# NetConnMon  [<img src="https://raw.githubusercontent.com/zachrybaker/NetConnMon/master/NetConnMon/Network-Router-icon.png" align="right" width="170">](https://github.com/zachrybaker/netconnmon)

Monitor your network connections while you work from home to diagnose connectivity issues.

This app also exists to give me an excuse to become versed in Blazor and some patterns.

## Installation
This app is intended to be run in a docker container. 

you can simply pull [the image](https://hub.docker.com/r/zachrybaker/netconnmon), if you prefer:

> docker pull zachrybaker/netconnmon

You need to set a mount/bind to a volume it expects to read at /app/netconnmon-db.

On the first run, it will create a database and config file in that location, which you can adjust as needed.  
They get recreated on next start if they are somehow deleted.

When the app is up, pull up the browser at port 80/443 and configure your tests!

## Coming soon:
* create test from protocol selection on list view.
* track email send error state in app (but not across app starts)
* Make the UI app a wasm not just a server-side app, by wrapping the server Api with a Rest Api ala https://github.com/mumby0168/blog-samples/tree/main/new-features/MinimalApis / https://github.com/jbogard/MediatR/issues/653
* finish authentication/authorization
    - Mudblazor-ify the identity UI pieces
    - turn it on in the layout
* WASM?
    - Add client API by swaping use of server API (change ref in _Imports.razor) for a client one, with the requests/commands moved to the domain project for reuse by both apps.
    - add the client-side handlers implemented via mediator with [refit](https://jonhilton.net/blazor-refit/) for the HTTP handling