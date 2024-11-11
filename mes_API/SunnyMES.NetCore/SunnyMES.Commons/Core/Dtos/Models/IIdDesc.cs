using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Commons.Core.Dtos.Models
{
    public interface IIdDesc
    {
        /// <summary>
        /// 描述ID
        /// </summary>
        [NotMapped]
        public int DefID { get; set; }

        /// <summary>
        /// 描述内容
        /// </summary>
        [NotMapped]
        public string DefDescription { get; set; }
    }
}
