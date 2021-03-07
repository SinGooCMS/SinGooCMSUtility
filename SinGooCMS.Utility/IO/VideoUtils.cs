using System;
using System.Collections.Generic;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 视频剪切工具.上传视频后可截取视频存储为图片,注意要调用文件：ffmpeg.exe
    /// </summary>
    public sealed class VideoUtils
    {
        #region 公共属性

        /// <summary>
        /// 工具所在路径
        /// </summary>
        public string ToolsPath { get; set; }
        /// <summary> 
        /// 截图起始时间段 格式 00:00:06
        /// </summary>
        public string StartPoint { get; set; }
        /// <summary>
        /// 截图长度
        /// </summary>
        public int CutImgWidth { get; set; }
        /// <summary>
        /// 截图宽度
        /// </summary>
        public int CutImgHeight { get; set; }
        /// <summary>
        /// 截图文件格式
        /// </summary>
        public string CutImgExtend { get; set; }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ffmpegPath">ffmpeg.exe的绝对路径</param>
        /// <param name="startPoint">视频开始时间 如00:00:06 表示第6秒 </param>
        /// <param name="cutImgWidth">截图宽 px</param>
        /// <param name="cutImgHeight">截图高 px</param>
        /// <param name="imgExt">截图扩展名</param>
        /// <returns>VideoUtils的实例</returns>
        public static VideoUtils Init(string ffmpegPath, string startPoint = "00:00:06", int cutImgWidth = 500, int cutImgHeight = 400, string imgExt = ".jpg")
        {
            return new VideoUtils
            {
                ToolsPath = ffmpegPath,
                StartPoint = startPoint,
                CutImgWidth = cutImgWidth,
                CutImgHeight = cutImgHeight,
                CutImgExtend = imgExt
            };
        }

        /// <summary>
        /// 保存截图
        /// </summary>
        /// <param name="fullVideoFile">视频文件路径</param>
        /// <param name="fullOutputImg">保存的图片文件路径</param>
        /// <returns></returns>
        public bool Save(string fullVideoFile, string fullOutputImg)
        {
            var commandTexts = new List<string>() {
                $"{ToolsPath} -i {fullVideoFile} -y -f image2 -ss {StartPoint} -t 0.001 -s {CutImgWidth} x {CutImgHeight} {fullOutputImg}"
            };
            return ProcessUtils.ExecuteCMD(commandTexts);
        }
    }
}
