using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace VRCToolBox.Common
{
    // reference:https://blog.okazuki.jp/entry/20100117/1263746037
    /// <summary>
    /// コマンド完了後に発生するイベントの引数
    /// </summary>
    public class CommandExecutedEventArgs : EventArgs
    {
        /// <summary>
        /// コマンドで発生したエラー。
        /// </summary>
        public Exception? Error { get; private set; }
        /// <summary>
        /// エラーを処理した場合はtrueに設定する
        /// </summary>
        public bool ErrorHandled { get; set; }


        public CommandExecutedEventArgs()
        {
        }
        public CommandExecutedEventArgs(Exception error)
        {
            Error = error;
        }
    }

    /// <summary>
    /// WindowとかのResourcesに登録して複数個所から参照されるのでFreezableを継承する。
    /// </summary>
    public class CommandReference : Freezable, ICommand
    {
        #region Execute実行前と実行後イベント
        public event EventHandler<CancelEventArgs> CommandExecuting;
        public event EventHandler<CommandExecutedEventArgs> CommandExecuted;
        #endregion

        public CommandReference()
        {
            // いちいちnullチェックするのがだるいので空ハンドラ登録しておく
            CommandExecuting += (a, b) => { };
            CommandExecuted += (a, b) => { };
        }

        // ViewModelのCommandとバインドするCommandプロパティ
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandReference), new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #region ICommand Members

        // ICommandの実装メソッド。実装内容は、タダ単に委譲しているだけ。
        public bool CanExecute(object? parameter) => Command != null && Command.CanExecute(parameter);
        public void Execute(object? parameter)
        {
            // コマンド実行前に実行できるかどうかイベントで確認する。
            var executingEventArgs = new CancelEventArgs();
            CommandExecuting(this, executingEventArgs);
            if (executingEventArgs.Cancel) return;
            try
            {
                // コマンド実行
                Command.Execute(parameter);
                // コマンド実行後イベント発行
                CommandExecuted(this, new CommandExecutedEventArgs());
            }
            catch (Exception ex)
            {
                // エラーがおきたときもコマンド実行後イベント発行
                var executedEventArgs = new CommandExecutedEventArgs(ex);
                CommandExecuted(this, executedEventArgs);
                // エラーが処理されてないようなら例外を再スロー
                if (!executedEventArgs.ErrorHandled) throw;
            }
        }

        public event EventHandler? CanExecuteChanged;


        // Commandプロパティが変ったタイミングで、イベントの登録先を入れ替える
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandReference? commandReference = d as CommandReference;
            ICommand? oldCommand = e.OldValue as ICommand;
            ICommand? newCommand = e.NewValue as ICommand;

            if (oldCommand != null) oldCommand.CanExecuteChanged -= commandReference?.CanExecuteChanged;
            if (newCommand != null) newCommand.CanExecuteChanged += commandReference?.CanExecuteChanged;
        }

        #endregion

        #region Freezable

        // 特にサポートしない
        protected override Freezable CreateInstanceCore()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    //public class CommandReference<T> : Freezable, ICommand
    //{
    //    #region Execute実行前と実行後イベント
    //    public event EventHandler<CancelEventArgs> CommandExecuting;
    //    public event EventHandler<CommandExecutedEventArgs> CommandExecuted;
    //    #endregion

    //    public CommandReference()
    //    {
    //        // いちいちnullチェックするのがだるいので空ハンドラ登録しておく
    //        CommandExecuting += (a, b) => { };
    //        CommandExecuted += (a, b) => { };
    //    }

    //    // ViewModelのCommandとバインドするCommandプロパティ
    //    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandReference<T>), new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));
    //    public static readonly DependencyProperty ParameterProperty = DependencyProperty.Register("CommandParameter", typeof(T), typeof(CommandReference<T>));
    //    public ICommand Command
    //    {
    //        get { return (ICommand)GetValue(CommandProperty); }
    //        set { SetValue(CommandProperty, value); }
    //    }
    //    public T CommandParameter
    //    {
    //        get { return (T)GetValue(ParameterProperty); }
    //        set { SetValue(ParameterProperty, value); }
    //    }
    //    #region ICommand Members

    //    // ICommandの実装メソッド。実装内容は、タダ単に委譲しているだけ。
    //    public bool CanExecute(object? parameter)
    //    {
    //        if (parameter is null) return Command is null;
    //        return Command != null && Command.CanExecute((T)parameter);
    //    }

    //    public void Execute(object? parameter)
    //    {
    //        // コマンド実行前に実行できるかどうかイベントで確認する。
    //        var executingEventArgs = new CancelEventArgs();
    //        CommandExecuting(this, executingEventArgs);
    //        if (executingEventArgs.Cancel || parameter is null) return;
    //        try
    //        {
    //            // コマンド実行
    //            Command.Execute((T)parameter);
    //            // コマンド実行後イベント発行
    //            CommandExecuted(this, new CommandExecutedEventArgs());
    //        }
    //        catch (Exception ex)
    //        {
    //            // エラーがおきたときもコマンド実行後イベント発行
    //            var executedEventArgs = new CommandExecutedEventArgs(ex);
    //            CommandExecuted(this, executedEventArgs);
    //            // エラーが処理されてないようなら例外を再スロー
    //            if (!executedEventArgs.ErrorHandled) throw;
    //        }
    //    }

    //    public event EventHandler? CanExecuteChanged;


    //    // Commandプロパティが変ったタイミングで、イベントの登録先を入れ替える
    //    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        CommandReference<T>? commandReference = d as CommandReference<T>;
    //        ICommand? oldCommand = e.OldValue as ICommand;
    //        ICommand? newCommand = e.NewValue as ICommand;

    //        if (oldCommand != null) oldCommand.CanExecuteChanged -= commandReference?.CanExecuteChanged;
    //        if (newCommand != null) newCommand.CanExecuteChanged += commandReference?.CanExecuteChanged;
    //    }

    //    #endregion

    //    #region Freezable

    //    // 特にサポートしない
    //    protected override Freezable CreateInstanceCore()
    //    {
    //        throw new NotSupportedException();
    //    }

    //    #endregion
    //}
}
