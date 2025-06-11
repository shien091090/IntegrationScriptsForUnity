#if CUSTOM_USING_ODIN
using System;
using SNShien.Common.ProcessTools;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface IViewManager : IArchitectureModel
    {
        int GetViewSortOrder<T>() where T : ArchitectureView;
        int GetViewSortOrder(Type type);
        void ClearAllView();
        void OpenView<T>(params object[] parameters) where T : ArchitectureView;
    }
}
#endif