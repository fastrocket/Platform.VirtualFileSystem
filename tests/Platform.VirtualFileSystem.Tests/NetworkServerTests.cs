﻿using NUnit.Framework;
using Platform.IO;
using Platform.VirtualFileSystem.Network.Server;

namespace Platform.VirtualFileSystem.Tests
{
	[TestFixture]
	[Category("IgnoreOnMono")]
	[Category("RequiresSockets")]
	public class NetworkServerTests
		: TestsBase
	{
		private INetworkFileSystemServer server;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			base.SetUp();

			this.server = TextBasedServer.CreateServer();

			this.server.Start();
			this.server.WaitForAnyTaskState(c => !(c == TaskState.Starting || c== TaskState.NotStarted));
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			this.server.Stop();
			this.server.Dispose();
		}

		[Test]
		public void Test_Connect_To_Server_And_Read_File()
		{
			var dir = FileSystemManager.Default.ResolveDirectory("netvfs://localhost[testfiles:///]/");

			dir.Refresh();

			Assert.IsTrue(dir.Exists);

			var file = dir.ResolveFile("TextFile1.txt");

			file.Refresh();

			Assert.IsTrue(file.Exists);

			Assert.AreEqual("TextFile1.txt", file.GetContent().GetReader().ReadToEndThenClose());

			var service = file.GetService<IFileHashingService>(new FileHashingServiceType());
			Assert.AreEqual("NetworkFileHashingService", service.GetType().Name);
			Assert.AreEqual("bec084d430670e66976d5abc24627a54", service.ComputeHash().TextValue);
		}
	}
}
