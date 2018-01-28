namespace rec CSharp.Mailboxes

open System 
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

[<Extension>]
type ActionExtensions private () =
    [<Extension>]
    static member ToFSharpFunc (a : Action<'t>) =
        fun p -> a.Invoke(p)
    [<Extension>]
    static member ToFSharpFunc (a : Action<'t1, 't2>) =
        fun p1 p2 -> a.Invoke (p1, p2)
    [<Extension>]
    static member ToFSharpFunc (a : Action<'t1, 't2, 't3>) =
        fun p1 p2 p3 -> a.Invoke (p1, p2, p3)
    [<Extension>]
    static member ToFSharpFunc (a : Action<'t1, 't2, 't3, 't4>) =
        fun p1 p2 p3 p4 -> a.Invoke (p1, p2, p3, p4)

[<Extension>]
type MessageProcessorExtensions private () = 
    [<Extension>]
    static member ReceiveAsync (mp : MailboxProcessor<'msg>) =
        mp.Receive() |> Async.StartAsTask
    [<Extension>]
    static member ReceiveAsync (mp : MailboxProcessor<'msg>, timeout: TimeSpan) =
        mp.Receive(timeout.TotalMilliseconds |> int32) |> Async.StartAsTask
    [<Extension>]
    member __.PostAndReplyAsync<'msg, 'reply>(mp : MailboxProcessor<'msg>, onReply: Func<AsyncReplyChannel<'reply>, 'msg>) =
        mp.PostAndAsyncReply<'reply> (fun ch -> onReply.Invoke(ch)) |> Async.StartAsTask 

type MailboxProcessor private() =
    static member Start (func: Func<Control.MailboxProcessor<'message>, Task>, ?timeout : CancellationToken) =
        let tm = defaultArg timeout (Unchecked.defaultof<CancellationToken>)
        Control.MailboxProcessor.Start 
            ((fun p -> 
                async {
                  do! Async.AwaitTask (func.Invoke(p))                  
               }), tm) 
    


