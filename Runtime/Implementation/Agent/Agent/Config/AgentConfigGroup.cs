using System.Collections.Generic;
using UnityEngine;

namespace XDay.AI
{
    [CreateAssetMenu(menuName = "XDay/AI/Agent/Group")]
    public class AgentConfigGroup : ScriptableObject
    {
        public string GroupName = "";
        /// <summary>
        /// 创建该group的config时默认保存目录
        /// </summary>
        public string ConfigCreateFolder = "";
        /// <summary>
        /// 创建该group的config的renderer时默认保存目录
        /// </summary>
        public string RendererCreateFolder = "";
        /// <summary>
        /// 创建config时同时创建一个Renderer Config
        /// </summary>
        public bool CreateRendererConfig = true;
        /// <summary>
        /// 删除Agent Config时同时删除Render Config
        /// </summary>
        public bool DeleteRendererConfig = true;
        public List<AgentConfig> Configs = new();
    }
}
