# UnityResourceManager
A Easy Unity Resource Managerment

---------------------------------------------------------------
2017-3-31
增加同步加载的接口，LoadAssetSync和LoadAllAssetSync
支持加载AllAssets的异步接口
增加RefAssetBundle的状态枚举，Unloaded，Loading，Loaded

---------------------------------------------------------------
2017-3-29
修改AssetBundleLoader的设计，改类只需要关注加载AssetBundle。
去除ResourceManager类，改为ResAgent类提供接口，类似Unity自身的Resource类。

---------------------------------------------------------------
2017-3-26
第一次上传Unity Resource ManagerMent模块，该模块主要宗旨是，用户不需要考虑资源的加载和卸载操作，只需要最简单的LoadAsset操作。

设计的理念仿照流水线的方式工作，由ResourceManager发起流程，RefAsset为整个流程中的被操作对象，最终用户得到的也是RefAsset对象。整个流水线中的工作流程，用户无需操心。

目前只针对AssetBundle的加载，使用接口为LoadFromFileAyncs。
未来将会支持的功能包括以下几点：
1.GameObject的对象池
2.内存警告后的紧急处理
3.加载接口支持WWW
4.支持同步异步的可选择
5.支持配置每个流水线阶段的限制参数(同时最大异步个数，读文件个数等等)

目前还存在的缺点：
1.用户在LoadAssetAsync后，如果不做任何处理，那么AssetBundle会残留内存，无法卸载。
2.代码结构还需要整理。
3.还不支持路径配置。
4.还不支持LoadAllAssets接口。
