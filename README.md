# TListener
* This is a functional listener provider library. You can instantiate a listener and listen a function. 
* 这是一个函数式的监听器类库。可以通过实例化一个监听器用于监听一个函数体。

# Sample 例子
``` CSharp
public class Program
{
    public static void Main(string[] args)
    {
        var lastDate = DateTime.Now;
        var listener = new TListener.Listener<string>()
                      .Listen(x =>
                       {
                           return true;
                       })
                      .Success(x =>
                       {
                           x.SyncContext.Post(s =>
                           {
                               Console.WriteLine(DateTime.Now.ToString());
                           }, null);
                       })
                      .Log(x => Console.WriteLine("Log - Times : " + x.Counter.Count().ToString()))
                      .Exit(x => Console.WriteLine("Exit"))
                      .Interval(5000)
                      .Times(5, x => Console.WriteLine("Times Out"))
                      .Build();
        listener.Start();
        Console.ReadKey();
    }
}
```
# Api Document
## IListener<TModel>
### Methods
#### Listen
``` CSharp
IListener<TModel> Listen(Func<ListenerContext<TModel>, bool>)
```
#### Success
``` CSharp
IListener<TModel> Success(Action<ListenerContext<TModel>>)
```
#### Error
``` CSharp
IListener<TModel> Error(Action<ListenerContext<TModel>, Exception>)
```
#### Log
``` CSharp
IListener<TModel> Log(Action<ListenerContext<TModel>>)
```
#### Interval
``` CSharp
IListener<TModel> Interval(int
```
#### Times
``` CSharp
IListener<TModel> Times(int, Action<ListenerContext<TModel>>?);
```
#### Exit
``` CSharp
IListener<TModel> Exit(Action<ListenerContext<TModel>>)
```
#### Build
``` CSharp
IListener<TModel> Build()
```
#### Start
``` CSharp
void Start()
```
#### Stop
``` CSharp
void Stop()
```

## ListenerContext
### Properties
#### SyncContext
``` CSharp
SynchronizationContext SyncContext
```
#### IsRunning
``` CSharp
bool IsRunning
```
#### TempData
``` CSharp
Hashtable TempData
```
#### WorkThread
``` CSharp
Thread WorkThread
```
#### Counter
``` CSharp
ICounter Counter
```
#### ListenerContext<<TModel>>.Model
```CSharp
TModel ListenerContext<TModel>.Model
```
### Methods
#### RequestStop
``` CSharp
void RequestStop()
```
