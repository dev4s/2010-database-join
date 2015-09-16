using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MySql.Data.MySqlClient;

namespace DatabaseJoin
{
	static class Program
	{
		static readonly MySqlConnection MySqlConPua = new MySqlConnection("empty");
		static readonly MySqlConnection MySqlConGrmg = new MySqlConnection("empty");

		private static readonly object Lockdb = new object();
		private static readonly object Locker = new object();
		private const int Threads = 4;
		private static Thread _t1;
		private static Thread _t2;
		private static Thread _t3;
		private static Thread _t4;

		private static readonly List<string> Errors = new List<string>();

		private static long _newUserId;
		private static long _newTopicId;
		private static long _newPostsId;
		private static long _newprivMsgsFolder;
		private static long _newprivMsgs;

		static void Main()
		{
			Time.StartAll();

			var grmgUsers = new List<DatabaseClasses.GrmgUsers>();
			var puaForums = new List<DatabaseClasses.PuaForums>();
			var puaUsers = new List<DatabaseClasses.PuaUsers>();
			var puaTopics = new List<DatabaseClasses.PuaTopics>();
			var puaTopicsPosted = new List<DatabaseClasses.PuaTopicsPosted>();
			var puaPrivMsg = new List<DatabaseClasses.PuaPrivmsgs>();
			var puaPrivMsgFolder = new List<DatabaseClasses.PuaPrivmsgsFolder>();
			var puaPrivMsgTo = new List<DatabaseClasses.PuaPrivmsgsTo>();
			var puaPosts = new List<DatabaseClasses.PuaPosts>();
			var puaAclGroups = new List<DatabaseClasses.AclGroups>();

			try
			{
				MySqlConPua.Open();
				MySqlConGrmg.Open();

				var pingThread = new Thread(PingDatabase);
				pingThread.Start();
			}
			catch (Exception)
			{
				Console.WriteLine("Błąd połączenia.");
				Console.ReadKey();
				return;
			}

			const string textGrmgForum = "phpbbnlp_forums";
			const string textPuaForum = "phpbb_forums";
			const string textGrmgUsers = "phpbbnlp_users";
			const string textPuaUsers = "phpbb_users";
			const string textGrmgTopics = "phpbbnlp_topics";
			const string textPuaTopics = "phpbb_topics";
			const string textGrmgTopicsPosted = "phpbbnlp_topics_posted";
			const string textPuaTopicsPosted = "phpbb_topics_posted";
			const string textGrmgPrivMsgs = "phpbbnlp_privmsgs";
			const string textPuaPrivMsgs = "phpbb_privmsgs";
			const string textGrmgPrivMsgsFolder = "phpbbnlp_privmsgs_folder";
			const string textPuaPrivMsgsFolder = "phpbb_privmsgs_folder";
			const string textGrmgPrivMsgsTo = "phpbbnlp_privmsgs_to";
			const string textPuaPrivMsgsTo = "phpbb_privmsgs_to";
			const string textGrmgPosts = "phpbbnlp_posts";
			const string textPuaPosts = "phpbb_posts";

			const string textGrmgAclGroups = "phpbbnlp_acl_groups";

			Console.WindowWidth = 110;

			Console.WriteLine("Pobieram: ");

			Time.Start();
			Console.Write("- wszystkie dane o użytkownikach z forum Pua");
			GetAllInformation(MySqlConPua, textPuaUsers, puaUsers);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- szczątkowe dane o użytkownikach z forum Grmg");
			GetSomeUserInformationFromGrmgUser(MySqlConGrmg, textGrmgUsers, grmgUsers);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- wszystkie dane o subforach z forum Pua");
			GetAllInformation(MySqlConPua, textPuaForum, puaForums);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- wszystkie tematy z forum Pua");
			GetAllInformation(MySqlConPua, textPuaTopics, puaTopics);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- wszystkie tematy zaakceptowane z forum Pua");
			GetAllInformation(MySqlConPua, textPuaTopicsPosted, puaTopicsPosted);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- wszystkie post'y z forum Pua");
			GetAllInformation(MySqlConPua, textPuaPosts, puaPosts);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- wszystkie priv'y z forum Pua");
			GetAllInformation(MySqlConPua, textPuaPrivMsgs, puaPrivMsg);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- wszystkie dane o folderach priv'ów z forum Pua");
			GetAllInformation(MySqlConPua, textPuaPrivMsgsFolder, puaPrivMsgFolder);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- wszystkie wysłane priv'y z forum Pua");
			GetAllInformation(MySqlConPua, textPuaPrivMsgsTo, puaPrivMsgTo);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- numer forum \"tymczasowego\" z forum Grmg");
			var temporaryForumId = GetTemporaryForumId(MySqlConGrmg, textGrmgForum);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- ostatni numer forum z forum Grmg");
			var forumIdNumber = GetLastNumberId(MySqlConGrmg, "forum_id", textGrmgForum);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- ostatni numer użytkownika z forum Grmg");
			_newUserId = GetLastNumberId(MySqlConGrmg, "user_id", textGrmgUsers);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- ostatni numer topic'a z forum Grmg");
			_newTopicId = GetLastNumberId(MySqlConGrmg, "topic_id", textGrmgTopics);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- ostatni numer post'a z forum Grmg");
			_newPostsId = GetLastNumberId(MySqlConGrmg, "post_id", textGrmgPosts);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- ostatni numer folderu priv'ów z forum Grmg");
			_newprivMsgsFolder = GetLastNumberId(MySqlConGrmg, "folder_id", textGrmgPrivMsgsFolder);
			HowLongDoesItTake();

			Time.Start();
			Console.Write("- ostatni numer priv'ów z forum Grmg");
			_newprivMsgs = GetLastNumberId(MySqlConGrmg, "msg_id", textGrmgPrivMsgs);
			HowLongDoesItTake();

			Console.WriteLine("\nZmieniam:");

			Time.Start();
			Console.Write("- numerację starego forum z Pua na nowe forum ");
			var lastNumber = GetLastNumberId(MySqlConGrmg, "right_id", textGrmgForum);
			ChangeForumNumeration(puaForums, forumIdNumber, temporaryForumId, lastNumber);
			CreateAclForForums(puaForums, puaAclGroups);
			HowLongDoesItTake();
			foreach (var forum in puaForums)
			{
				Console.WriteLine(String.Format("  = Nowe id: {0}, nowy parent_id: {1}, left_id: {2}, right_id: {3}", forum.new_forum_id, forum.new_parent_id, forum.new_left_id, forum.new_right_id));
			}

			Time.Start();
			Console.Write("- i całą resztę tałatajstwa - dodatkowo wpisując na serwer\n");

			//warunki sprawdzeniowe
			InsertUsersIntoDatabase(puaUsers, textGrmgUsers, grmgUsers);

			//wpisanie forów do bazy
			InsertForumsIntoDatabase(puaForums, textGrmgForum);
			foreach (var group in puaAclGroups)
			{
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgAclGroups, group);
			}

			//wpisanie tematów do bazy
			InsertTopicsIntoDatabase(textGrmgTopics, puaForums, puaTopics, puaUsers);

			//wpisanie postów do bazy
			InsertPostsIntoDatabase(puaUsers, textGrmgPosts, puaForums, puaTopics, puaPosts);

			//wpisanie folderu privów do bazy
			InsertPrivMsgFolderIntoDatabase(puaUsers, puaPrivMsgFolder, textGrmgPrivMsgsFolder);

			//wpisanie privów do bazy
			InsertPrivMsgIntoDatabase(puaUsers, puaPrivMsg, textGrmgPrivMsgs);

			//wpisanie privów wysłanych do bazy
			InsertPrivMsgToIntoDatabase(puaUsers, puaPrivMsg, puaPrivMsgFolder, puaPrivMsgTo, textGrmgPrivMsgsTo);

			//wpisanie topiców wysłanych
			InsertTopicsPostedIntoDatabase(puaUsers, puaTopics, puaTopicsPosted, textGrmgTopicsPosted);

			Console.WriteLine("\nBłędy:");
			foreach (var error in Errors)
			{
				Console.WriteLine(" - " + error);
			}

			Console.Write("\n\nZakończono, czas potrzebny: " + Time.StopAll());
			Console.ReadKey();
		}

		private static void CreateAclForForums(IEnumerable<DatabaseClasses.PuaForums> puaForums, ICollection<DatabaseClasses.AclGroups> puaAclGroups)
		{
			foreach (var forum in puaForums)
			{
				puaAclGroups.Add(new DatabaseClasses.AclGroups
				                 	{
										new_forum_id = forum.new_forum_id,
				                 		group_id = 12776,
				                 		forum_id = forum.new_forum_id,
										auth_option_id = 0,
										auth_role_id = 29,
										auth_setting = 0
									});

				puaAclGroups.Add(new DatabaseClasses.AclGroups
									{
										new_forum_id = forum.new_forum_id,
										group_id = 12774,
										forum_id = forum.new_forum_id,
										auth_option_id = 0,
										auth_role_id = 32,
										auth_setting = 0
									});

				puaAclGroups.Add(new DatabaseClasses.AclGroups
									{
										new_forum_id = forum.new_forum_id,
										group_id = 12773,
										forum_id = forum.new_forum_id,
										auth_option_id = 0,
										auth_role_id = 32,
										auth_setting = 0
									});

				puaAclGroups.Add(new DatabaseClasses.AclGroups
									{
										new_forum_id = forum.new_forum_id,
										group_id = 12777,
										forum_id = forum.new_forum_id,
										auth_option_id = 0,
										auth_role_id = 29,
										auth_setting = 0
									});

				puaAclGroups.Add(new DatabaseClasses.AclGroups
									{
										new_forum_id = forum.new_forum_id,
										group_id = 12776,
										forum_id = forum.new_forum_id,
										auth_option_id = 0,
										auth_role_id = 27,
										auth_setting = 0
									});

				puaAclGroups.Add(new DatabaseClasses.AclGroups
									{
										new_forum_id = forum.new_forum_id,
										group_id = 12777,
										forum_id = forum.new_forum_id,
										auth_option_id = 0,
										auth_role_id = 27,
										auth_setting = 0
									});
			}

		}

		private static void InsertTopicsPostedIntoDatabase(IEnumerable<DatabaseClasses.PuaUsers> puaUsers, IEnumerable<DatabaseClasses.PuaTopics> puaTopics, IList<DatabaseClasses.PuaTopicsPosted> puaTopicsPosted, string textGrmgTopicsPosted)
		{
			var tempLength = puaTopicsPosted.Count;
			var piece = tempLength / Threads;
			var piece2 = piece + piece;
			var piece3 = piece2 + piece;

			_t1 = new Thread(() => TopicsPostedChanges(0, piece, puaTopicsPosted, puaUsers, puaTopics));
			_t2 = new Thread(() => TopicsPostedChanges(piece, piece2, puaTopicsPosted, puaUsers, puaTopics));
			_t3 = new Thread(() => TopicsPostedChanges(piece2, piece3, puaTopicsPosted, puaUsers, puaTopics));
			_t4 = new Thread(() => TopicsPostedChanges(piece3, tempLength, puaTopicsPosted, puaUsers, puaTopics));
			_t1.Start();
			_t2.Start();
			_t3.Start();
			_t4.Start();

			while (_t1.ThreadState == ThreadState.Running || _t2.ThreadState == ThreadState.Running || _t3.ThreadState == ThreadState.Running || _t4.ThreadState == ThreadState.Running)
			{
				Thread.Sleep(1000);
			}
			_t1 = _t2 = _t3 = _t4 = null;


			for (var i = 0; i < puaTopicsPosted.Count; i++)
			{
				Console.WriteLine(String.Format("- wpisano topicsposted {0} z {1}", i, puaTopicsPosted.Count));
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgTopicsPosted, puaTopicsPosted[i]);
			}
		}

		private static void InsertPrivMsgToIntoDatabase(IEnumerable<DatabaseClasses.PuaUsers> puaUsers, IEnumerable<DatabaseClasses.PuaPrivmsgs> puaPrivMsg, IEnumerable<DatabaseClasses.PuaPrivmsgsFolder> puaPrivMsgFolder, IList<DatabaseClasses.PuaPrivmsgsTo> puaPrivMsgTo, string textGrmgPrivMsgsTo)
		{
			var tempLength = puaPrivMsgTo.Count;
			var piece = tempLength / Threads;
			var piece2 = piece + piece;
			var piece3 = piece2 + piece;

			_t1 = new Thread(() => PrivMsgsToChanges(0, piece, puaPrivMsgTo, puaUsers, puaPrivMsg, puaPrivMsgFolder));
			_t2 = new Thread(() => PrivMsgsToChanges(piece, piece2, puaPrivMsgTo, puaUsers, puaPrivMsg, puaPrivMsgFolder));
			_t3 = new Thread(() => PrivMsgsToChanges(piece2, piece3, puaPrivMsgTo, puaUsers, puaPrivMsg, puaPrivMsgFolder));
			_t4 = new Thread(() => PrivMsgsToChanges(piece3, tempLength, puaPrivMsgTo, puaUsers, puaPrivMsg, puaPrivMsgFolder));
			_t1.Start();
			_t2.Start();
			_t3.Start();
			_t4.Start();

			while (_t1.ThreadState == ThreadState.Running || _t2.ThreadState == ThreadState.Running || _t3.ThreadState == ThreadState.Running || _t4.ThreadState == ThreadState.Running)
			{
				Thread.Sleep(1000);
			}
			_t1 = _t2 = _t3 = _t4 = null;

			for (var i = 0; i < puaPrivMsgTo.Count; i++)
			{
				Console.WriteLine(String.Format("- wpisano privto nr {0} z {1}", i, puaPrivMsgTo.Count));
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgPrivMsgsTo, puaPrivMsgTo[i]);
			}
		}

		private static void InsertPrivMsgIntoDatabase(IEnumerable<DatabaseClasses.PuaUsers> puaUsers, IList<DatabaseClasses.PuaPrivmsgs> puaPrivMsg, string textGrmgPrivMsgs)
		{
			if (puaPrivMsg == null) throw new ArgumentNullException("puaPrivMsg");
			var tempLength = puaPrivMsg.Count;
			var piece = tempLength / Threads;
			var piece2 = piece + piece;
			var piece3 = piece2 + piece;

			_t1 = new Thread(() => PrivMsgsChanges(0, piece, puaPrivMsg, puaUsers));
			_t2 = new Thread(() => PrivMsgsChanges(piece, piece2, puaPrivMsg, puaUsers));
			_t3 = new Thread(() => PrivMsgsChanges(piece2, piece3, puaPrivMsg, puaUsers));
			_t4 = new Thread(() => PrivMsgsChanges(piece3, tempLength, puaPrivMsg, puaUsers));
			_t1.Start();
			_t2.Start();
			_t3.Start();
			_t4.Start();

			while (_t1.ThreadState == ThreadState.Running || _t2.ThreadState == ThreadState.Running || _t3.ThreadState == ThreadState.Running || _t4.ThreadState == ThreadState.Running)
			{
				Thread.Sleep(1000);
			}
			_t1 = _t2 = _t3 = _t4 = null;

			for (var i = 0; i < puaPrivMsg.Count; i++)
			{
				Console.WriteLine(String.Format("- wpisano privmsg nr {0} z {1}", i, puaPrivMsg.Count));
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgPrivMsgs, puaPrivMsg[i]);
			}
		}

		private static void InsertPrivMsgFolderIntoDatabase(IEnumerable<DatabaseClasses.PuaUsers> puaUsers, IList<DatabaseClasses.PuaPrivmsgsFolder> puaPrivMsgFolder, string textGrmgPrivMsgsFolder)
		{
			var tempLength = puaPrivMsgFolder.Count;
			var piece = tempLength / Threads;
			var piece2 = piece + piece;
			var piece3 = piece2 + piece;

			_t1 = new Thread(() => PrivMsgsFolderChanges(0, piece, puaPrivMsgFolder, puaUsers));
			_t2 = new Thread(() => PrivMsgsFolderChanges(piece, piece2, puaPrivMsgFolder, puaUsers));
			_t3 = new Thread(() => PrivMsgsFolderChanges(piece2, piece3, puaPrivMsgFolder, puaUsers));
			_t4 = new Thread(() => PrivMsgsFolderChanges(piece3, tempLength, puaPrivMsgFolder, puaUsers));
			_t1.Start();
			_t2.Start();
			_t3.Start();
			_t4.Start();

			while (_t1.ThreadState == ThreadState.Running || _t2.ThreadState == ThreadState.Running || _t3.ThreadState == ThreadState.Running || _t4.ThreadState == ThreadState.Running)
			{
				Thread.Sleep(1000);
			}
			_t1 = _t2 = _t3 = _t4 = null;

			for (var i = 0; i < puaPrivMsgFolder.Count; i++)
			{
				Console.WriteLine(String.Format("- wpisano privfolder nr {0} z {1}", i, puaPrivMsgFolder.Count));
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgPrivMsgsFolder, puaPrivMsgFolder[i]);
			}
		}

		private static void InsertPostsIntoDatabase(IEnumerable<DatabaseClasses.PuaUsers> puaUsers, string textGrmgPosts, IEnumerable<DatabaseClasses.PuaForums> puaForums, IEnumerable<DatabaseClasses.PuaTopics> puaTopics, IList<DatabaseClasses.PuaPosts> puaPosts)
		{
			var tempLength = puaPosts.Count;
			var piece = tempLength / Threads;
			var piece2 = piece + piece;
			var piece3 = piece2 + piece;

			_t1 = new Thread(() => PostsChanges(0, piece, puaPosts, puaUsers, puaForums, puaTopics));
			_t2 = new Thread(() => PostsChanges(piece, piece2, puaPosts, puaUsers, puaForums, puaTopics));
			_t3 = new Thread(() => PostsChanges(piece2, piece3, puaPosts, puaUsers, puaForums, puaTopics));
			_t4 = new Thread(() => PostsChanges(piece3, tempLength, puaPosts, puaUsers, puaForums, puaTopics));
			_t1.Start();
			_t2.Start();
			_t3.Start();
			_t4.Start();

			while (_t1.ThreadState == ThreadState.Running || _t2.ThreadState == ThreadState.Running || _t3.ThreadState == ThreadState.Running || _t4.ThreadState == ThreadState.Running)
			{
				Thread.Sleep(1000);
			}
			_t1 = _t2 = _t3 = _t4 = null;

			for (var i = 0; i < puaPosts.Count; i++)
			{
				Console.WriteLine(String.Format("- wpisano post nr {0} z {1}", i, puaPosts.Count));
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgPosts, puaPosts[i]);
			}
		}

		private static void InsertTopicsIntoDatabase(string textGrmgTopics, IEnumerable<DatabaseClasses.PuaForums> puaForums, IList<DatabaseClasses.PuaTopics> puaTopics, IEnumerable<DatabaseClasses.PuaUsers> puaUsers)
		{
			var tempLength = puaTopics.Count;
			var piece = tempLength / Threads;
			var piece2 = piece + piece;
			var piece3 = piece2 + piece;
			_t1 = new Thread(() => TopicChanges(0, piece, puaTopics, puaForums, puaUsers));
			_t2 = new Thread(() => TopicChanges(piece, piece2, puaTopics, puaForums, puaUsers));
			_t3 = new Thread(() => TopicChanges(piece2, piece3, puaTopics, puaForums, puaUsers));
			_t4 = new Thread(() => TopicChanges(piece3, tempLength, puaTopics, puaForums, puaUsers));
			_t1.Start();
			_t2.Start();
			_t3.Start();
			_t4.Start();

			while (_t1.ThreadState == ThreadState.Running || _t2.ThreadState == ThreadState.Running || _t3.ThreadState == ThreadState.Running || _t4.ThreadState == ThreadState.Running)
			{
				Thread.Sleep(1000);
			}
			_t1 = _t2 = _t3 = _t4 = null;


			for (var index = 0; index < puaTopics.Count; index++)
			{
				Console.WriteLine(String.Format("- wpisano topic nr {0} z {1} z id {2}", index, puaTopics.Count, puaTopics[index].new_topic_id));
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgTopics, puaTopics[index]);
			}
		}

		private static void InsertForumsIntoDatabase(IEnumerable<DatabaseClasses.PuaForums> puaForums, string textGrmgForum)
		{
			foreach (var forum in puaForums)
			{
				Console.WriteLine(String.Format("- wpisano forum ({0}) do bazy", forum.forum_name));
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgForum, forum);
			}
		}

		private static void InsertUsersIntoDatabase(IEnumerable<DatabaseClasses.PuaUsers> puaUsers, string textGrmgUsers, IList<DatabaseClasses.GrmgUsers> grmgUsers)
		{
			var tempLength = grmgUsers.Count;
			var piece = tempLength / Threads;
			var piece2 = piece + piece;
			var piece3 = piece2 + piece;

			_t1 = new Thread(() => UsersChanges(0, piece, puaUsers, grmgUsers));
			_t2 = new Thread(() => UsersChanges(piece, piece2, puaUsers, grmgUsers));
			_t3 = new Thread(() => UsersChanges(piece2, piece3, puaUsers, grmgUsers));
			_t4 = new Thread(() => UsersChanges(piece3, tempLength, puaUsers, grmgUsers));
			_t1.Start();
			_t2.Start();
			_t3.Start();
			_t4.Start();

			while (_t1.ThreadState == ThreadState.Running || _t2.ThreadState == ThreadState.Running || _t3.ThreadState == ThreadState.Running || _t4.ThreadState == ThreadState.Running)
			{
				Thread.Sleep(1000);
			}
			_t1 = _t2 = _t3 = _t4 = null;

			//wpisanie userów do bazy
			var usersInGrmg = (from gu in grmgUsers from pu in puaUsers where pu.new_user_id == gu.user_id select pu).Distinct().ToList();
			var usersNotInGrmg = puaUsers.Except(usersInGrmg).ToList();

			foreach (var user in usersNotInGrmg)
			{
				Console.WriteLine(String.Format("- wpisano użytkownika ({0}) do bazy", user.username_clean));
				InsertIntoGrmgDatabase(MySqlConGrmg, textGrmgUsers, user);
			}
		}

		private static void PingDatabase()
		{
			while (true)
			{
				lock (Lockdb)
				{
					MySqlConPua.Ping();
					MySqlConGrmg.Ping();
				}
				Thread.Sleep(500);
			}
		}

		private static void TopicsPostedChanges(int from, int to, IList<DatabaseClasses.PuaTopicsPosted> puaTopicsPosted, IEnumerable<DatabaseClasses.PuaUsers> puaUsers, IEnumerable<DatabaseClasses.PuaTopics> puaTopics)
		{
			for (; from < to; from++)
			{
				foreach (var user in puaUsers)
				{
					if (puaTopicsPosted[from].user_id == user.user_id)
					{
						lock (Locker)
						{
							puaTopicsPosted[from].new_user_id = user.new_user_id;
						}
					}
				}

				foreach (var topic in puaTopics)
				{
					if (puaTopicsPosted[from].topic_id == topic.topic_id)
					{
						lock (Locker)
						{
							puaTopicsPosted[from].new_topic_id = topic.new_topic_id;
						}
					}
				}
			}
		}

		private static void PrivMsgsToChanges(int from, int to, IList<DatabaseClasses.PuaPrivmsgsTo> puaPrivMsgTo, IEnumerable<DatabaseClasses.PuaUsers> puaUsers, IEnumerable<DatabaseClasses.PuaPrivmsgs> puaPrivMsg, IEnumerable<DatabaseClasses.PuaPrivmsgsFolder> puaPrivMsgFolder)
		{
			for (; from < to; from++)
			{
				foreach (var privmsg in puaPrivMsg)
				{
					if (puaPrivMsgTo[from].msg_id == privmsg.msg_id)
					{
						lock (Locker)
						{
							puaPrivMsgTo[from].new_msg_id = privmsg.new_msg_id;
						}
					}
				}

				foreach (var user in puaUsers)
				{
					if (puaPrivMsgTo[from].author_id == user.user_id)
					{
						lock (Locker)
						{
							puaPrivMsgTo[from].new_author_id = user.new_user_id;
						}
					}

					if (puaPrivMsgTo[from].user_id != user.user_id) continue;
					lock (Locker)
					{
						puaPrivMsgTo[from].new_user_id = user.new_user_id;
					}
				}

				foreach (var folder in puaPrivMsgFolder)
				{
					if (puaPrivMsgTo[from].folder_id == folder.folder_id)
					{
						lock (Locker)
						{
							puaPrivMsgTo[from].new_folder_id = folder.new_folder_id;
						}
					}
				}
			}
		}

		private static void PrivMsgsChanges(int from, int to, IList<DatabaseClasses.PuaPrivmsgs> puaPrivMsg, IEnumerable<DatabaseClasses.PuaUsers> puaUsers)
		{
			for (; from < to; from++)
			{
				lock (Locker)
				{
					puaPrivMsg[from].new_msg_id = ++_newprivMsgs;
				}

				foreach (var user in puaUsers)
				{
					if (puaPrivMsg[from].author_id == user.user_id)
					{
						lock (Locker)
						{
							puaPrivMsg[from].new_author_id = user.new_user_id;
						}
					}
				}
			}
		}

		private static void PrivMsgsFolderChanges(int from, int to, IList<DatabaseClasses.PuaPrivmsgsFolder> puaPrivMsgFolder, IEnumerable<DatabaseClasses.PuaUsers> puaUsers)
		{
			for (; from < to; from++)
			{
				lock (Locker)
				{
					puaPrivMsgFolder[from].new_folder_id = ++_newprivMsgsFolder;
				}

				foreach (var user in puaUsers)
				{
					if (puaPrivMsgFolder[from].user_id == user.user_id)
					{
						lock (Locker)
						{
							puaPrivMsgFolder[from].new_user_id = user.new_user_id;
						}
					}
				}
			}
		}

		private static void PostsChanges(int from, int to, IList<DatabaseClasses.PuaPosts> puaPosts, IEnumerable<DatabaseClasses.PuaUsers> puaUsers, IEnumerable<DatabaseClasses.PuaForums> puaForums, IEnumerable<DatabaseClasses.PuaTopics> puaTopics)
		{
			for (; from < to; from++)
			{
				//zmiana numeracji
				lock (Locker)
				{
					puaPosts[from].new_post_id = ++_newPostsId;
				}

				//podpięcie pod użytkownika
				foreach (var user in puaUsers)
				{
					if (puaPosts[from].poster_id == user.user_id)
					{
						lock (Locker)
						{
							puaPosts[from].new_poster_id = user.new_user_id;
						}
					}

					if(puaPosts[from].post_edit_user == user.user_id)
					{
						lock (Locker)
						{
							puaPosts[from].post_edit_user = user.new_user_id;
							puaPosts[from].post_username = user.username;
						}
					}
				}

				//podpięcie pod forum
				foreach (var forum in puaForums)
				{
					if(puaPosts[from].forum_id == forum.forum_id)
					{
						lock (Locker)
						{
							puaPosts[from].new_forum_id = forum.new_forum_id;
						}
					}
				}

				//podpięcie pod topic
				foreach (var topic in puaTopics)
				{
					if (puaPosts[from].topic_id == topic.topic_id)
					{
						lock (Locker)
						{
							puaPosts[from].new_topic_id = topic.new_topic_id;
						}
					}
				}

				foreach (var topic in puaTopics)
				{
					if (topic.topic_last_post_id == puaPosts[from].post_id)
					{
						lock (Locker)
						{
							topic.topic_last_post_id = puaPosts[from].new_post_id;
							topic.topic_last_post_subject = puaPosts[from].post_subject;
						}
					}
					if (topic.topic_first_post_id == puaPosts[from].post_id)
					{
						lock (Locker)
						{
							topic.topic_first_post_id = puaPosts[from].new_post_id;
						}
					}
				}
			}
		}

		private static void TopicChanges(int from, int to, IList<DatabaseClasses.PuaTopics> puaTopics, IEnumerable<DatabaseClasses.PuaForums> puaForums, IEnumerable<DatabaseClasses.PuaUsers> puaUsers)
		{
			for (; from < to; from++)
			{
				lock (Locker)
				{
					puaTopics[from].new_topic_id = ++_newTopicId;
				}

				foreach (var forum in puaForums)
				{
					if (puaTopics[from].forum_id == forum.forum_id)
					{
						lock (Locker)
						{
							puaTopics[from].new_forum_id = forum.new_forum_id;
						}
					}
				}

				foreach (var user in puaUsers)
				{
					if (puaTopics[from].topic_last_poster_id == user.user_id)
					{
						lock (Locker)
						{
							puaTopics[from].topic_last_poster_id = user.new_user_id;
							puaTopics[from].topic_last_poster_name = user.username;
						}
					}

					if (puaTopics[from].topic_poster == user.user_id)
					{
						lock (Locker)
						{
							puaTopics[from].topic_poster = user.new_user_id;
						}
					}

					if (puaTopics[from].topic_first_post_id == user.user_id)
					{
						lock (Locker)
						{
							puaTopics[from].topic_first_post_id = user.new_user_id;
							puaTopics[from].topic_first_poster_name = user.username;
						}
					}
				}

				lock (Locker)
				{
					puaTopics[from].topic_moved_id = 0;
				}

				if (puaTopics[from].new_topic_id != 0 && puaTopics[from].new_topic_id != 1) continue;
				lock (Locker)
				{
					puaTopics[from].new_topic_id = ++_newTopicId;
				}
			}
		}

		private static void UsersChanges(int from, int to, IEnumerable<DatabaseClasses.PuaUsers> puaUsers, IList<DatabaseClasses.GrmgUsers> grmgUsers)
		{
			foreach (var puaUser in puaUsers)
			{
				var tempfrom = from;

				for (; from < to; from++)
				{
					if (puaUser.username_clean == "domin")
					{
						lock (Locker)
						{
							puaUser.username = "Projekt PUA";
							puaUser.username_clean = "projekt pua";
							puaUser.new_user_id = ++_newUserId;	
						}
					}
					else if (puaUser.username_clean == grmgUsers[from].username_clean)
					{
						if (puaUser.user_email == grmgUsers[from].user_email)
						{
							lock (Locker)
							{
								puaUser.new_user_id = grmgUsers[from].user_id;
							}
						}
						else
						{
							lock (Locker)
							{
								puaUser.username += "_2";
								puaUser.username_clean += "_2";
								puaUser.new_user_id = ++_newUserId;
							}
						}
					}
					else
					{
						if (puaUser.user_email == grmgUsers[from].user_email)
						{
							lock (Locker)
							{
								puaUser.new_user_id = grmgUsers[from].user_id;
							}
						}
					}
				}

				from = tempfrom;

				//zarejestrowani użytkownicy
				lock (Locker)
				{
					puaUser.group_id = 12774;
				}

				if (puaUser.new_user_id != 0 && puaUser.new_user_id != 1) continue;
				lock (Locker)
				{
					puaUser.new_user_id = ++_newUserId;
				}
			}
		}

		private static void InsertIntoGrmgDatabase(MySqlConnection connection, string databaseTable, object databaseData)
		{
			lock (Lockdb)
			{
				var columnNames = GetProperties(databaseData);
				var values = GetValues(databaseData);
				var commandText = String.Format("insert into {0} ({1}) values ({2})", databaseTable, columnNames, values);
				var command = new MySqlCommand(commandText, connection);
				try
				{
					command.ExecuteNonQuery();
				}
				catch(Exception e)
				{
					Errors.Add(e.Message);
				}
			}
		}

		private static int GetLastNumberId(MySqlConnection connection, string columnName, string databaseTable)
		{
			lock (Lockdb)
			{
				var commandText = String.Format("select max({0}) from {1}", columnName, databaseTable);
				var command = new MySqlCommand(commandText, connection);
				var reader = command.ExecuteReader();
				reader.Read();
				int result;
				try
				{
					result = reader.GetInt32(0);
				}
				catch (Exception)
				{
					result = 1;
				}
				reader.Close();
				return result;
			}
		}

		private static void GetAllInformation<T>(MySqlConnection connection, string databaseTable, ICollection<T> values) where T : new()
		{
			lock (Lockdb)
			{
				var command = new MySqlCommand("select * from " + databaseTable, connection);
				var reader = command.ExecuteReader();

				//var counter = 0;

				while (reader.Read())
				{
					//counter++;
					values.Add((T)GetProperties(reader, new T()));
					//if (counter == 100) break;
				}

				reader.Close();
			}
		}

		private static void ChangeForumNumeration(IEnumerable<DatabaseClasses.PuaForums> phpbbForums, int forumIdNumber, int temporaryForumId, int lastNumber)
		{
			foreach (var forum in phpbbForums)
			{
				//error in forums
				if (forum.new_right_id == 0)
				{
					var tempDiff = forum.right_id - forum.left_id;
					if (tempDiff > 1)
					{
						forum.new_left_id = lastNumber;

						var forum1 = forum;
						var temporaryForum = phpbbForums.Where(x => x.left_id > forum1.left_id && x.right_id < forum1.right_id);

						foreach (var t in temporaryForum)
						{
							t.new_left_id = ++lastNumber;
							t.new_right_id = ++lastNumber;
						}

						forum.new_right_id = ++lastNumber;
					}
					else
					{
						forum.new_left_id = ++lastNumber;
						forum.new_right_id = ++lastNumber;
					}
				}

				//temporary - should work
				forum.forum_last_post_id = 0;
				forum.forum_last_poster_id = 0;

				forum.new_forum_id = ++forumIdNumber;

				if (forum.parent_id == 0)
					forum.new_parent_id = temporaryForumId;
			}

			phpbbForums.Single(x => x.parent_id != 0).new_parent_id = phpbbForums.Join(phpbbForums, p1 => p1.parent_id, p2 => p2.forum_id, (p1, p2) => p2.new_forum_id).SingleOrDefault();
		}

		private static int GetTemporaryForumId(MySqlConnection connection, string databaseTable)
		{
			lock (Lockdb)
			{
				var temporaryForumIdCommand = new MySqlCommand(String.Format("select forum_id from {0} where forum_name like '%ymczasow%'", databaseTable), connection);
				var temporaryForumIdReader = temporaryForumIdCommand.ExecuteReader();
				temporaryForumIdReader.Read();
				var temporaryForumId = temporaryForumIdReader.GetInt32(0);
				temporaryForumIdReader.Close();
				return temporaryForumId;
			}
			
		}

		private static void GetSomeUserInformationFromGrmgUser(MySqlConnection connection, string userForum, ICollection<DatabaseClasses.GrmgUsers> grmgUsers)
		{
			lock(Lockdb) 
			{
				var command = new MySqlCommand("select user_id, group_id, username, username_clean, user_password, user_email from " + userForum, connection);
				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					grmgUsers.Add((DatabaseClasses.GrmgUsers) GetProperties(reader, new DatabaseClasses.GrmgUsers()));
				}
				reader.Close();
			}
		}

		private static string GetValues(Object temp)
		{
			var propertyInfo = Type.GetType(temp.ToString()).GetProperties();
			var result = "";

			foreach (var info in propertyInfo)
			{

				const string nameRegex = "(\\Auser_id\\Z|\\Aforum_id\\Z|\\Aparent_id\\Z|\\Atopic_id\\Z|\\Aposter_id\\Z|\\Apost_id\\Z|\\Afolder_id\\Z|\\Amsg_id\\Z|\\Aauthor_id\\Z|\\Aleft_id\\Z|\\Aright_id\\Z)";

				if (!new Regex(nameRegex).Match(info.Name).Success)
				{
					object tempResult;
					switch (info.PropertyType.Name)
					{
						case "Int64":
							tempResult = info.GetValue(temp, null);
							result += tempResult + ", ";
							break;
						case "Int32":
							tempResult = info.GetValue(temp, null);
							result += tempResult + ", ";
							break;
						case "String":
							tempResult = info.GetValue(temp, null);
							var regex = new Regex("'");
							var regexResult = regex.Replace(tempResult.ToString(), "");
							result += "'" + regexResult + "', ";
							break;
					}
				}
			}
			return result.Remove(result.Length - 2);
		}

		private static string GetProperties(Object temp)
		{
			var propertyInfo = Type.GetType(temp.ToString()).GetProperties();

			var result = propertyInfo.Where(info => !info.Name.StartsWith("new_")).Aggregate("", (current, info) => current + (info.Name + ", "));
			return result.Remove(result.Length - 2);
		}

		private static object GetProperties(MySqlDataReader mySqlReader, object temp)
		{
			var propertyInfos = Type.GetType(temp.ToString()).GetProperties();

			foreach (var propertyInfo in propertyInfos)
			{
				switch (propertyInfo.PropertyType.Name)
				{
					case "Int64":
						try 
						{
							propertyInfo.SetValue(temp, mySqlReader.GetInt64(propertyInfo.Name), null);
						}
						catch(Exception)
						{
							propertyInfo.SetValue(temp, 0, null);
						}
						break;
					case "String":
						try
						{
							propertyInfo.SetValue(temp, mySqlReader.GetString(propertyInfo.Name), null);
						}
						catch (Exception)
						{
							propertyInfo.SetValue(temp, "", null);
						}
						break;
				}
			}
			return temp;
		}

		private static void HowLongDoesItTake()
		{
			Console.WriteLine(" ...zakończono. Czas potrzebny: " + Time.Stop());
		}

		static class Time
		{
			private static DateTime _startTime;
			private static DateTime _startAllTime;

			public static void Start()
			{
				_startTime = DateTime.Now;
			}

			public static TimeSpan Stop()
			{
				return DateTime.Now - _startTime;
			}

			public static void StartAll()
			{
				_startAllTime = DateTime.Now;
			}

			public static TimeSpan StopAll()
			{
				return DateTime.Now - _startAllTime;
			}
		}
	}
}
