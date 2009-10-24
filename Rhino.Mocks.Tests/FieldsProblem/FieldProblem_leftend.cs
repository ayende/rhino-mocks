using System;
using Xunit;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_leftend : IDisposable
	{
		private MockRepository mocks;
		private IAddAlbumPresenter viewMock;
		private IAlbum albumMock;
		private IEventRaiser saveRaiser;

		public FieldProblem_leftend()
		{
			mocks = new MockRepository();
			viewMock =
				(IAddAlbumPresenter) mocks.DynamicMock(typeof (IAddAlbumPresenter));
			albumMock = mocks.StrictMock<IAlbum>();

			viewMock.Save += null;
			LastCall.IgnoreArguments().Constraints(Is.NotNull());
			saveRaiser = LastCall.GetEventRaiser();
		}

		public void Dispose()
		{
			mocks.VerifyAll();
		}

		[Fact]
		public void VerifyAttachesToViewEvents()
		{
			mocks.ReplayAll();
			new AddAlbumPresenter(viewMock);
		}

		[Fact]
		public void SaveEventShouldSetViewPropertiesCorrectly()
		{
			Expect.Call(viewMock.AlbumToSave).Return(albumMock);
			albumMock.Save();//create expectation
			viewMock.ProcessSaveComplete();//create expectation
			mocks.ReplayAll();

			AddAlbumPresenter presenter = new
				AddAlbumPresenter(viewMock);
			saveRaiser.Raise(null, null);
		}

		public interface IAlbum
		{
			string Name { get; set; }
			void Save();
		}

		public class Album : IAlbum
		{
			private string mName;

			public string Name
			{
				get { return mName; }
				set { mName = value; }
			}

			public Album()
			{
			}

			public void Save()
			{
				//code to save to db
			}
		}

		public interface IAddAlbumPresenter
		{
			IAlbum AlbumToSave { get; }
			event EventHandler<EventArgs> Save;
			void ProcessSaveComplete();
		}

		public class AddAlbumPresenter
		{
			private IAddAlbumPresenter mView;

			public AddAlbumPresenter(IAddAlbumPresenter view)
			{
				mView = view;
				Initialize();
			}

			private void Initialize()
			{
				mView.Save += new
					EventHandler<EventArgs>(mView_Save);
			}

			private void mView_Save(object sender, EventArgs e)
			{
				IAlbum newAlbum = mView.AlbumToSave;
				try
				{
					newAlbum.Save();
					mView.ProcessSaveComplete();
				}
				catch
				{
					//handle exception
				}
			}
		}
	}
}