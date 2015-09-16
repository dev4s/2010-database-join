namespace DatabaseJoin
{
	static class DatabaseClasses
	{
		public class PuaPosts
		{
			public long new_post_id { get; set; }
			public long new_topic_id { get; set; }
			public long new_forum_id { get; set; }
			public long new_poster_id { get; set; }

			public long post_id { get; set; }
			public long topic_id { get; set; }
			public long forum_id { get; set; }
			public long poster_id { get; set; }
			public long icon_id { get; set; }
			public string poster_ip { get; set; }
			public long post_time { get; set; }
			public long post_approved { get; set; }
			public long post_reported { get; set; }
			public long enable_bbcode { get; set; }
			public long enable_smilies { get; set; }
			public long enable_magic_url { get; set; }
			public long enable_sig { get; set; }
			public string post_username { get; set; }
			public string post_subject { get; set; }
			public string post_text { get; set; }
			public string post_checksum { get; set; }
			public long post_attachment { get; set; }
			public string bbcode_bitfield { get; set; }
			public string bbcode_uid { get; set; }
			public long post_postcount { get; set; }
			public long post_edit_time { get; set; }
			public string post_edit_reason { get; set; }
			public long post_edit_user { get; set; }
			public long post_edit_count { get; set; }
			public long post_edit_locked { get; set; }
		}
		public class PuaPrivmsgsTo
		{
			public long new_msg_id { get; set; }
			public long new_user_id { get; set; }
			public long new_author_id { get; set; }
			public long new_folder_id { get; set; }

			public long msg_id { get; set; }
			public long user_id { get; set; }
			public long author_id { get; set; }
			public long folder_id { get; set; }
			public long pm_deleted { get; set; }
			public long pm_new { get; set; }
			public long pm_unread { get; set; }
			public long pm_replied { get; set; }
			public long pm_marked { get; set; }
			public long pm_forwarded { get; set; }
		}
		public class PuaPrivmsgsFolder
		{
			public long new_folder_id { get; set;}
			public long new_user_id { get; set; }

			public long folder_id { get; set; }
			public long user_id { get; set; }
			public string folder_name { get; set; }
			public long pm_count { get; set; }
		}
		public class PuaPrivmsgs
		{
			public long new_msg_id { get; set; }
			public long new_author_id { get; set; }

			public long msg_id { get; set; }
			public long author_id { get; set; }
			public long root_level { get; set; }
			public long icon_id { get; set; }
			public string author_ip { get; set; }
			public long message_time { get; set; }
			public long enable_bbcode { get; set; }
			public long enable_smilies { get; set; }
			public long enable_magic_url { get; set; }
			public long enable_sig { get; set; }
			public string message_subject { get; set; }
			public string message_text { get; set; }
			public string message_edit_reason { get; set; }
			public long message_edit_user { get; set; }
			public long message_attachment { get; set; }
			public string bbcode_bitfield { get; set; }
			public string bbcode_uid { get; set; }
			public long message_edit_time { get; set; }
			public long message_edit_count { get; set; }
			public string to_address { get; set; }
			public string bcc_address { get; set; }
		}
		public class PuaTopicsPosted
		{
			public long new_user_id { get; set; }
			public long new_topic_id { get; set; }

			public long user_id { get; set; }
			public long topic_id { get; set; }
			public long topic_posted { get; set; }
		}
		public class PuaTopics
		{
			public long new_topic_id { get; set; }
			public long new_forum_id { get; set; }

			public long topic_id { get; set; }
			public long forum_id { get; set; }
			public long icon_id { get; set; }
			public long topic_attachment { get; set; }
			public long topic_approved { get; set; }
			public long topic_reported { get; set; }
			public string topic_title { get; set; }
			public long topic_poster { get; set; }
			public long topic_time { get; set; }
			public long topic_time_limit { get; set; }
			public long topic_views { get; set; }
			public long topic_replies { get; set; }
			public long topic_replies_real { get; set; }
			public long topic_status { get; set; }
			public long topic_type { get; set; }
			public long topic_first_post_id { get; set; }
			public string topic_first_poster_name { get; set; }
			public string topic_first_poster_colour { get; set; }
			public long topic_last_post_id { get; set; }
			public long topic_last_poster_id { get; set; }
			public string topic_last_poster_name { get; set; }
			public string topic_last_poster_colour { get; set; }
			public string topic_last_post_subject { get; set; }
			public long topic_last_post_time { get; set; }
			public long topic_last_view_time { get; set; }
			public long topic_moved_id { get; set; }
			public long topic_bumped { get; set; }
			public long topic_bumper { get; set; }
			public string poll_title { get; set; }
			public long poll_start { get; set; }
			public long poll_length { get; set; }
			public long poll_max_options { get; set; }
			public long poll_last_vote { get; set; }
			public long poll_vote_change { get; set; }
		}
		public class PuaUsers
		{
			public long new_user_id { get; set; }

			public long user_id { get; set; }
			public long user_type { get; set; }
			public long group_id { get; set; }
			public string user_permissions { get; set; }
			public long user_perm_from { get; set; }
			public string user_ip { get; set; }
			public long user_regdate { get; set; }
			public string username { get; set; }
			public string username_clean { get; set; }
			public string user_password { get; set; }
			public long user_passchg { get; set; }
			public long user_pass_convert { get; set; }
			public string user_email { get; set; }
			public long user_email_hash { get; set; }
			public string user_birthday { get; set; }
			public long user_lastvisit { get; set; }
			public long user_lastmark { get; set; }
			public long user_lastpost_time { get; set; }
			public string user_lastpage { get; set; }
			public string user_last_confirm_key { get; set; }
			public long user_last_search { get; set; }
			public long user_warnings { get; set; }
			public long user_last_warning { get; set; }
			public long user_login_attempts { get; set; }
			public long user_inactive_reason { get; set; }
			public long user_inactive_time { get; set; }
			public long user_posts { get; set; }
			public string user_lang { get; set; }
			public long user_timezone { get; set; }
			public long user_dst { get; set; }
			public string user_dateformat { get; set; }
			public long user_style { get; set; }
			public long user_rank { get; set; }
			public string user_colour { get; set; }
			public long user_new_privmsg { get; set; }
			public long user_unread_privmsg { get; set; }
			public long user_last_privmsg { get; set; }
			public long user_message_rules { get; set; }
			public long user_full_folder { get; set; }
			public long user_emailtime { get; set; }
			public long user_topic_show_days { get; set; }
			public string user_topic_sortby_type { get; set; }
			public string user_topic_sortby_dir { get; set; }
			public long user_post_show_days { get; set; }
			public string user_post_sortby_type { get; set; }
			public string user_post_sortby_dir { get; set; }
			public long user_notify { get; set; }
			public long user_notify_pm { get; set; }
			public long user_notify_type { get; set; }
			public long user_allow_pm { get; set; }
			public long user_allow_viewonline { get; set; }
			public long user_allow_viewemail { get; set; }
			public long user_allow_massemail { get; set; }
			public long user_options { get; set; }
			public string user_avatar { get; set; }
			public long user_avatar_type { get; set; }
			public long user_avatar_width { get; set; }
			public long user_avatar_height { get; set; }
			public string user_sig { get; set; }
			public string user_sig_bbcode_uid { get; set; }
			public string user_sig_bbcode_bitfield { get; set; }
			public string user_from { get; set; }
			public string user_icq { get; set; }
			public string user_aim { get; set; }
			public string user_yim { get; set; }
			public string user_msnm { get; set; }
			public string user_jabber { get; set; }
			public string user_website { get; set; }
			public string user_occ { get; set; }
			public string user_interests { get; set; }
			public string user_actkey { get; set; }
			public string user_newpasswd { get; set; }
			public string user_form_salt { get; set; }
		}
		public class PuaForums
		{
			public long new_forum_id { get; set; }
			public long new_parent_id { get; set; }
			public long new_left_id { get; set; }
			public long new_right_id { get; set; }

			public long forum_id { get; set; }
			public long parent_id { get; set; }
			public long left_id { get; set; }
			public long right_id { get; set; }
			public string forum_parents { get; set; }
			public string forum_name { get; set; }
			public string forum_desc { get; set; }
			public string forum_desc_bitfield { get; set; }
			public long forum_desc_options { get; set; }
			public string forum_desc_uid { get; set; }
			public string forum_link { get; set; }
			public string forum_password { get; set; }
			public long forum_style { get; set; }
			public string forum_image { get; set; }
			public string forum_rules { get; set; }
			public string forum_rules_link { get; set; }
			public string forum_rules_bitfield { get; set; }
			public long forum_rules_options { get; set; }
			public string forum_rules_uid { get; set; }
			public long forum_topics_per_page { get; set; }
			public long forum_type { get; set; }
			public long forum_status { get; set; }
			public long forum_posts { get; set; }
			public long forum_topics { get; set; }
			public long forum_topics_real { get; set; }
			public long forum_last_post_id { get; set; }
			public long forum_last_poster_id { get; set; }
			public string forum_last_post_subject { get; set; }
			public long forum_last_post_time { get; set; }
			public string forum_last_poster_name { get; set; }
			public string forum_last_poster_colour { get; set; }
			public long forum_flags { get; set; }
			public long display_subforum_list { get; set; }
			public long display_on_index { get; set; }
			public long enable_indexing { get; set; }
			public long enable_icons { get; set; }
			public long enable_prune { get; set; }
			public long prune_next { get; set; }
			public long prune_days { get; set; }
			public long prune_viewed { get; set; }
			public long prune_freq { get; set; }
		}
		public class AclGroups
		{
			public long new_forum_id { get; set; }

			public long forum_id { get; set; }
			public long group_id { get; set; }	
			public long auth_option_id { get; set; }
			public long auth_role_id { get; set; }
			public long auth_setting { get; set; }
		}

		public class GrmgUsers
		{
			public long user_id { get; set; }
			public string username { get; set; }
			public string username_clean { get; set; }
			public string user_email { get; set; }
		}
		
	}
}
