namespace MongoRequestHelper.Models.Enums
{
    /// <summary>
    /// Действия с привилегиями
    /// </summary>
    public static class PrivilegeActions
    {
        // Query and Write Actions

        public static string Find = "find";

        public static string Insert = "insert";

        public static string Remove = "remove";

        public static string Update = "update";

        public static string BypassDocumentValidation = "bypassDocumentValidation";

        public static string UseUuid = "useUUID";

        // Database Management Actions

        public static string ChangeCustomData = "changeCustomData";

        public static string ChangeOwnCustomData = "changeOwnCustomData";

        public static string ChangeOwnPassword = "changeOwnPassword";

        public static string ChangePassword = "changePassword";

        public static string CreateCollection = "createCollection";

        public static string CreateIndex = "createIndex";

        public static string CreateRole = "createRole";

        public static string CreateUser = "createUser";

        public static string DropCollection = "dropCollection";

        public static string DropRole = "dropRole";

        public static string DropUser = "dropUser";

        public static string EnableProfiler = "enableProfiler";

        public static string GrantRole = "grantRole";

        public static string KillCursors = "killCursors";

        public static string KillAnyCursor = "killAnyCursor";

        public static string PlanCacheIndexFilter = "planCacheIndexFilter";

        public static string RevokeRole = "revokeRole";

        public static string SetAuthenticationRestriction = "setAuthenticationRestriction";

        public static string SetFeatureCompatibilityVersion = "setFeatureCompatibilityVersion";

        public static string Unlock = "unlock";

        public static string ViewRole = "viewRole";

        public static string ViewUser = "viewUser";

        // Deployment Management Actions

        public static string AuthSchemaUpgrade = "authSchemaUpgrade";

        public static string CleanupOrphaned = "cleanupOrphaned";

        public static string CpuProfiler = "cpuProfiler";

        public static string Inprog = "inprog";

        public static string InvalidateUserCache = "invalidateUserCache";

        public static string Killop = "killop";

        public static string PlanCacheRead = "planCacheRead";

        public static string PlanCacheWrite = "planCacheWrite";

        public static string StorageDetails = "storageDetails";

        // Change Stream Actions

        public static string ChangeStream = "changeStream";

        // Replication Actions

        public static string AppendOplogNote = "appendOplogNote";

        public static string ReplSetConfigure = "replSetConfigure";

        public static string ReplSetGetConfig = "replSetGetConfig";

        public static string ReplSetGetStatus = "replSetGetStatus";

        public static string ReplSetHeartbeat = "replSetHeartbeat";

        public static string ReplSetStateChange = "replSetStateChange";

        public static string Resync = "resync";

        // Sharding Actions

        public static string AddShard = "addShard";

        public static string ClearJumboFlag = "clearJumboFlag";

        public static string EnableSharding = "enableSharding";

        public static string RefineCollectionShardKey = "refineCollectionShardKey";

        public static string FlushRouterConfig = "flushRouterConfig";

        public static string GetShardMap = "getShardMap";

        public static string GetShardVersion = "getShardVersion";

        public static string ListShards = "listShards";

        public static string MoveChunk = "moveChunk";

        public static string RemoveShard = "removeShard";

        public static string ShardingState = "shardingState";

        public static string SplitVector = "splitVector";

        // Server Administration Actions

        public static string ApplicationMessage = "applicationMessage";

        public static string CloseAllDatabases = "closeAllDatabases";

        public static string CollMod = "collMod";

        public static string Compact = "compact";

        public static string ConnPoolSync = "connPoolSync";

        public static string ConvertToCapped = "convertToCapped";

        public static string DropConnections = "dropConnections";

        public static string DropDatabase = "dropDatabase";

        public static string DropIndex = "dropIndex";

        public static string ForceUuid = "forceUUID";

        public static string Fsync = "fsync";

        public static string GetDefaultRwConcern = "getDefaultRWConcern";

        public static string GetParameter = "getParameter";

        public static string HostInfo = "hostInfo";

        public static string OidReset = "oidReset";

        public static string LogRotate = "logRotate";

        public static string ReIndex = "reIndex";

        public static string RenameCollectionSameDb = "renameCollectionSameDB";

        public static string RotateCertificates = "rotateCertificates";

        public static string SetDefaultRwConcern = "setDefaultRWConcern";

        public static string SetParameter = "setParameter";

        public static string Shutdown = "shutdown";

        public static string Touch = "touch";

        // Session Actions

        public static string Impersonate = "impersonate";

        public static string ListSessions = "listSessions";

        public static string KillAnySession = "killAnySession";

        // Free Monitoring Actions

        public static string CheckFreeMonitoringStatus = "checkFreeMonitoringStatus";

        public static string SetFreeMonitoring = "setFreeMonitoring";

        // Diagnostic Actions

        public static string CollStats = "collStats";

        public static string ConnPoolStats = "connPoolStats";

        public static string CursorInfo = "cursorInfo";

        public static string DbHash = "dbHash";

        public static string DbStats = "dbStats";

        public static string GetCmdLineOpts = "getCmdLineOpts";

        public static string GetLog = "getLog";

        public static string IndexStats = "indexStats";

        public static string ListDatabases = "listDatabases";

        public static string ListCollections = "listCollections";

        public static string ListIndexes = "listIndexes";

        public static string Netstat = "netstat";

        public static string ServerStatus = "serverStatus";

        public static string Validate = "validate";

        public static string Top = "top";

        // Internal Actions

        public static string AnyAction = "anyAction";

        public static string Internal = "internal";

        public static string ApplyOps = "applyOps";
    }
}