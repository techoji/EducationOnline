using System;

namespace DistanceLearning.Models.TeacherUpdates {
    public class TopicModel {

        public int CourseId { get; set; }
        public String SectionName { get; set; }
        public String Topic { get; set; }
        public String Description { get; set; }
        public bool IsHidden { get; set; }

        //public String GetClearedSectionName() => RemoveTags(SectionName);
        //public String GetClearedTopic() => RemoveTags(Topic);
        //public String GetClearedDescription() => RemoveTags(Description);

        //private string RemoveTags(String htmlText) {
        //    if (String.IsNullOrWhiteSpace(htmlText))
        //        return String.Empty;
        //    string[] skipTags = { "b", "i", "br", "em", "strong", "h1", "h2", "h3", "h4", "h5", "h6", "span", "p" };
        //    String htmlTextRes;
        //    if ((skipTags == null) || (skipTags.Length == 0)) {
        //        Regex tagsExp = new Regex(@"(?><\s*(?:/|!)?\s*\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)\s*>", RegexOptions.IgnoreCase);
        //        htmlTextRes = tagsExp.Replace(htmlText, String.Empty);
        //    }
        //    else {
        //        Regex tagsExp = new Regex(@"(?><\s*(?:/|!)?\s*(\w+))(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)\s*>", RegexOptions.IgnoreCase);
        //        MatchEvaluator tagexcluder = match => {
        //            if (match.Groups.Count > 1) {
        //                string tag = match.Groups[1].Value;
        //                if (skipTags.Contains(tag, StringComparer.InvariantCultureIgnoreCase))
        //                    return match.Value;
        //            }
        //            return String.Empty;
        //        };
        //        htmlTextRes = tagsExp.Replace(htmlText, tagexcluder);
        //    }
        //    return htmlTextRes;
        //}

    }
}