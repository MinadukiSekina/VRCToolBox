using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VRCToolBox.Common
{

    public sealed class NotifyTaskCompletion : INotifyPropertyChanged
    {
        public NotifyTaskCompletion(Task task)
        {
            TargetTask = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }
        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                  new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
            }
        }
        public Task TargetTask { get; private set; }

        public TaskStatus Status { get { return TargetTask.Status; } }
        public bool IsCompleted { get { return TargetTask.IsCompleted; } }
        public bool IsNotCompleted { get { return !TargetTask.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return TargetTask.Status == TaskStatus.RanToCompletion;
            }
        }
        public bool IsCanceled { get { return TargetTask.IsCanceled; } }
        public bool IsFaulted { get { return TargetTask.IsFaulted; } }
        public AggregateException? Exception => TargetTask.Exception;
        public Exception? InnerException
        {
            get
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }
        public string? ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public sealed class NotifyTaskCompletion<T> : INotifyPropertyChanged
    {
        public NotifyTaskCompletion(Task<T> task)
        {
            TargetTask = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }
        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                  new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this,
                  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }
        public Task<T> TargetTask { get; private set; }
        public T? Result
        {
            get
            {
                return (TargetTask.Status == TaskStatus.RanToCompletion) ? TargetTask.Result : default(T);
            }
        }
        public TaskStatus Status { get { return TargetTask.Status; } }
        public bool IsCompleted { get { return TargetTask.IsCompleted; } }
        public bool IsNotCompleted { get { return !TargetTask.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return TargetTask.Status == TaskStatus.RanToCompletion;
            }
        }
        public bool IsCanceled { get { return TargetTask.IsCanceled; } }
        public bool IsFaulted { get { return TargetTask.IsFaulted; } }
        public AggregateException? Exception => TargetTask.Exception;
        public Exception? InnerException
        {
            get
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }
        public string? ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
