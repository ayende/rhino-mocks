using System;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.MethodRecorders
{
	/// <summary>
	/// Change the recorder from ordered to unordered and vice versa
	/// </summary>
	public class RecorderChanger : IDisposable
	{
		private IMethodRecorder recorder;
		private readonly MockRepository repository;

		/// <summary>
		/// Creates a new <see cref="RecorderChanger"/> instance.
		/// </summary>
		public RecorderChanger(MockRepository repository, IMethodRecorder recorder, IMethodRecorder newRecorder)
		{
			this.recorder = recorder;
			this.repository = repository;
			repository.PushRecorder(newRecorder);
			this.recorder.AddRecorder(newRecorder);
		}


		/// <summary>
		/// Disposes this instance.
		/// </summary>
		public void Dispose()
		{
			this.recorder.MoveToPreviousRecorder();
			repository.PopRecorder();
		}
	}
}