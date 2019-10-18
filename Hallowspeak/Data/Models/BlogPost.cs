using System;
using Markdig;
using Markdig.SyntaxHighlighting;
using Microsoft.AspNetCore.Components;

namespace Hallowspeak.Data.Models
{
    public class BlogPost
    {
        public int Id { get; private set; }
        public string Title { get; set; }
        public string Excerpt { get => _excerpt.TrimEnd() + "..."; set => _excerpt = value.Length <= 255 ? value : value.Substring(0, 255); }
        public DateTime Date_GMT { get => _date.ToUniversalTime(); set => _date = value; }
        public string Content { get; set; }
        /// <summary>
        /// Must be URL
        /// </summary>
        public string Thumbnail { get; set; }
        private DateTime _date;
        private string _excerpt;

        public BlogPost(int id, string title, string content, string excerpt = "", string thumbnail = "/img/Thumbnail.png", DateTime? date_GMT = null)
        {
            Id = id;
            Title = title;
            Excerpt = excerpt;
            Date_GMT = date_GMT ?? DateTime.Now;
            Thumbnail = thumbnail;
            Content = content;
        }
    }
}
