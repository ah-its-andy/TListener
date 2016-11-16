# TListener
<p> This is a functional listener provider library. You can instantiate a listener and listen a function. </p>
<p> 这是一个函数式的监听器类库。可以通过实例化一个监听器用于监听一个函数体。</p>

# Sample 例子
``` CSharp
public class Program
{
    public static void Main(string[] args)
    {
        var lastDate = DateTime.Now;
        var listener = new TListener.Listener<string>()
                      .Listen(x => /*匿名参数是一个ListenerContext对象*/
                       {
                           return true; /*监听器将循环执行此函数，直到函数体返回true，则跳转至Success中的函数体*/
                       })
                      .Success(x =>
                       {
                           x.SyncContext /*SyncContext可以用于向主线程发送函数体*/
                            .Post(s =>
                            {
                                Console.WriteLine(DateTime.Now.ToString()); /*Post中的操作会切换到主线程执行*/
                            }, null);
                       })
                      .Log(x => Console.WriteLine("Log - Times : " + x.Counter.Count().ToString())) /*监听函数体每次执行都将在函数最后执行Log函数体*/
                      .Exit(x => Console.WriteLine("Exit"))/*当监听器结束监听时，运行此函数体*/
                      .Interval(5000)/*监听器执行监听函数体的间隔*/
                      .Times(5, x => Console.WriteLine("Times Out"))/*监听器Success函数得次数，并在次数达到后执行函数体*/
                      .Build();/*构建监听器，在开始监听前必须先构建监听器*/
        listener.Start();/*开始监听*/
        Console.ReadKey();
    }
}
```
# Api Document
## IListener<TModel>
### Methods
#### Listen
``` CSharp
IListener<TModel> Listen(Func<ListenerContext<TModel>, bool> _handler)
```
<p>设置监听操作，当函数返回true则执行Success，如果抛出异常则执行Error.</p>
<em>参数</em>
<p><em>_handler</em> ：监听函数体</p>

#### Success
``` CSharp
IListener<TModel> Success(Action<ListenerContext<TModel>> _handler)
```
<p>当监听函数返回true时，执行此函数体.</p>
<em>参数</em>
<p><em>_handler</em> ：函数体</p>

#### Error
``` CSharp
IListener<TModel> Error(Action<ListenerContext<TModel>, Exception> _handler)
```
<p>当监听函数抛出异常时，执行此函数体.</p>
<em>参数</em>
<p><em>_handler</em> ：异常处理函数体</p>

#### Log
``` CSharp
IListener<TModel> Log(Action<ListenerContext<TModel>> _handler)
```
<p>每当执行监听函数后，执行此函数.</p>
<em>参数</em>
<p><em>_handler</em> ：日志函数体</p>

#### Interval
``` CSharp
IListener<TModel> Interval(int _interval)
```
<p>当执行监听函数后，线程将挂起一段时间.</p>
<em>参数</em>
<p><em>_interval</em> ：线程挂起的事件</p>

#### Times
``` CSharp
IListener<TModel> Times(int times, Action<ListenerContext<TModel>>? _handler);
```
<p>设置一个值,指示Success函数执行的最大次数.</p>
<em>参数</em>
<p><em>times</em> ：Success函数执行的最大次数.</p>
<p><em>?_handler</em> ：当Success函数执行次数达到最大次数时,执行此函数.</p>

#### Exit
``` CSharp
IListener<TModel> Exit(Action<ListenerContext<TModel>> _handler)
```
<p>当监听器退出时，执行此函数体.</p>
<em>参数</em>
<p><em>_handler</em> ：函数体</p>

#### Build
``` CSharp
IListener<TModel> Build()
```
<p>构建监听器,必须先构建监听器才能启动.</p>

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
