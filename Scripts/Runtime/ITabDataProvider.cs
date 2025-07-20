using System;

namespace UGC.Tabview
{
    /// <summary>
    /// 标签页数据提供者接口
    /// </summary>
    public interface ITabDataProvider
    {
        /// <summary>
        /// 获取指定ID的数据
        /// </summary>
        /// <param name="dataId">数据ID</param>
        /// <returns>数据对象</returns>
        object GetData(string dataId);
        
        /// <summary>
        /// 刷新指定ID的数据
        /// </summary>
        /// <param name="dataId">数据ID</param>
        void RefreshData(string dataId);
    }
}