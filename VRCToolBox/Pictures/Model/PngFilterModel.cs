using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class PngFilterModel : IPngFilterModel
    {
        /// <summary>
        /// 親のモデルを保持
        /// </summary>
        private IPngEncoderOptions _encoderOptions;

        /// <summary>
        /// フィルター処理
        /// </summary>
        private PngFilter _filter;

        internal PngFilterModel(IPngEncoderOptions pngEncoderOptions, PngFilter pngFilter)
        {
            _encoderOptions = pngEncoderOptions;
            _filter         = pngFilter;
        }

        /// <summary>
        /// 親のモデルの値に対して自分のフラグ値を加える or 削除する
        /// </summary>
        /// <param name="isAdd"></param>
        private void ModifyFilterOption(bool isAdd)
        {
            if (isAdd)
            {
                _encoderOptions.AddFilterOption(_filter);
            }
            else
            {
                _encoderOptions.RemoveFilterOption(_filter);
            }
        }

        PngFilter IPngFilterModel.FilterValue => _filter;

        void IPngFilterModel.ModifyFilterOption(bool isAdd) => ModifyFilterOption(isAdd);
    }
}
