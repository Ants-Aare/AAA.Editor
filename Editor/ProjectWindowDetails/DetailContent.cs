using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowDetails
{
    public struct DetailContent
    {
        public string DetailText;
        public string DetailTooltip;
        public Color DetailColor;
        public static DetailContent Empty = new DetailContent("");
        public DetailContent(string detailText, string detailTooltip = "")
        {
            DetailText = detailText;
            this.DetailColor = Color.white;
            this.DetailTooltip = detailTooltip;
        }
        public DetailContent(string detailText, Color detailColor, string detailTooltip = "")
        {
            DetailText = detailText;
            this.DetailTooltip = detailTooltip;
            this.DetailColor = detailColor;
        }
    }
}
