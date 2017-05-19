namespace LostTech.App
{
	using System;
	using System.ComponentModel;
	using System.Diagnostics.Contracts;
	using System.Threading.Tasks;

	/// <summary>
	/// Wraps a <see cref="Task"/>, and signals its completion as <see cref="INotifyPropertyChanged"/>
	/// </summary>
	/// <typeparam name="T">Type of the task result value</typeparam>
	public sealed class BindableTask<T> : INotifyPropertyChanged
	{
		/// <summary>
		/// Creates new instance of <see cref="BindableTask{T}"/>
		/// </summary>
		public BindableTask(Task<T> task)
		{
			Contract.Requires(task != null);

			this.Task = task;

			if (!task.IsCompleted)
			{
				this.Await(task);
			}
		}

		async void Await(Task<T> task)
		{
			try
			{
				await task;
			}
			catch(Exception)
			{
				this.OnPropertyChanged(nameof(this.Exception));
			}

			this.OnPropertyChanged(nameof(this.Value));
			this.OnPropertyChanged(nameof(this.IsPending));
		}

		public Task<T> Task { get; }
		public T Value => this.Task.IsCompleted ? this.Task.Result : default(T);
		public Exception Exception => this.Task.IsCompleted ? this.Task.Exception : null;
		public bool IsPending => !(this.Task.IsCompleted || this.Task.IsCanceled || this.Task.IsFaulted);

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public static implicit operator BindableTask<T>(Task<T> task) => new BindableTask<T>(task);
	}
}
