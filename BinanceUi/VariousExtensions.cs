using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BinanceUi;

public static class VariousExtensions
{
    public static IDisposable DisposeWith(this IDisposable disposable, CompositeDisposable compositeDisposable)
    {
        if (compositeDisposable == null)
            throw new ArgumentNullException(nameof(compositeDisposable));
        if (disposable == null)
            throw new ArgumentNullException(nameof(disposable));

        compositeDisposable.Add(disposable);
        return disposable;
    }
}