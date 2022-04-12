# 异步编程与async&await关键字

C# 异步编程基于Task任务模型，包括异步方法 `async` `await` 关键字  

## Task任务模型

在 .NET Core 中使用 Task 任务模型所需的所有对象都集中在 `System.Threading.Tasks` 命名空间中。  
最常用到的是 `Task` 类及其泛型类 `Task<TResult>`，创建任务即初始化 `Task` 类型，可以赋值给变量，通过 `Task.Run` 静态方法给信号开始执行任务代码，然后执行其他不依赖该任务的代码。

## 异步方法
定义异步方法可以在声明方法时使用 `async` 关键字声明这是一个异步方法。按照约定，异步方法以Async结尾。返回值可以选用 `void` `Task` `Task<TResult>`，如果要像同步方法一样返回 `void` 一般直接使用 `Task` 类作为返回值。要返回引用类型或值类型时使用 `Task<TResult>`，其中 `TResult` 类比同步方法中的返回值。要异步处理事件的时候使用 `void` 作为返回值。返回值为 `void` 时不能 `await`。

## `async` 以及 `await` 关键字
`async` 及 `await` 是C#中异步编程的核心，其中 `async` 定义异步方法，`await` 处理异步调用。  
`await` 关键字只能在声明了 `async` 的异步方法中使用，用来等待一个 `Task` 完成，如果有的话把该任务的返回值作为 `await` 表达式的值。在等待任务完成的时候会把控制流交回给调用方，调用方再去做其他不依赖该异步方法的任务。同理，异步方法在开始一个异步任务 `TaskA` 的时候，可以去做其他不依赖 `TaskA` 的工作。在需要其结果时，才使用 `await` 关键字，如果 `TaskA` 也使用了 `await` 等待其他工作，那么控制流交回给该方法。

```C#
public async void main() {
    //....
    var result = await AsyncMethod();
    // 这种写法会丧失异步编程的优势
    // 一般提前开始任务再在需要的时候 await
    Consolo.Write(result);
}

public async Task<string> AsyncMethod(){
    var taskA = TaskA();
    //....
    // 不依赖 TaskA 的工作
    // 可能与 TaskA 一起执行(乱序执行)
    //....
    await taskA; // 需要该任务完成才可继续
    Sleep(2333);
    return "awa";
}

public async Task TaskA(){
    //....
    // 异步工作，将使用 await 关键字
    //....
    return;
}
```

控制流从 `main` 方法开始，执行到异步调用，同时挂起任务结果，控制流进入 `AsyncMethod`，开始一个任务，如果是单线程环境则会乱序执行 `AsyncMethod` 以及 `TaskA`，等到 `AsyncMethod` 执行到 `await` 时挂起， `TaskA` 完成返回之后再恢复 `AsyncMethod`。注意在这个过程中，`AsyncMethod` 执行到 `await` 会将控制流再返回调用方，乱序执行调用方及其他任务(`TaskA`)以更好的利用算力资源。  
多线程环境下的理想情况是同时有两条线程在执行这两个任务，然后等 `AsyncMethod` 执行到 `await` 时挂起之后把控制流返回给调用方(`main`)执行如果有的，不依赖 `AsyncMethod` 的其他工作。等到 `await` 返回之后再恢复 `AsyncMethod` 执行剩下的工作。

整理一下流程，控制流从 `main` 到 `AsyncMethod` 然后开始 `TaskA`，此时如果 `TaskA` 执行到 `await`，那么控制流会返回到 `AsyncMethod` 执行不依赖 `TaskA` 的工作，等到 `TaskA` 遇到的 `await` 完成后再恢复 `TaskA`。`AsyncMethod` 中的 `await` 以及 `await` 前的代码同理。  
在所有异步方法中，遇到 `await` 之前的任务都是可以跟其他异步任务一起执行，也就是乱序执行不同任务尽可能利用更多算力资源以达到更高的性能。