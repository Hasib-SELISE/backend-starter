namespace RecruitingMoms.Common.Constants;

public class RecruitingMomsServiceConstants
{
    public static string TenantId = "986A938D-81CA-4D87-8DAC-7AB1470BBC48";

        #region RecruitingMomsStore

        public const string RecruitingMomsStoreChangeQueueName = "RecruitingMomsStoreChangeQueue";

        #endregion

        #region EmpMgmt

        public const string EmployeeInfoChangeEventQueueName = "EmployeeInfoChangeEventQueue";
        public const string GenEmpMgmtEmployeeInfoChangeEventExchangeName = "Selise.Ecap.Server.EmployeeMgmt:EmployeeInfoChangeEvent";

        #endregion

        #region Academy

        public const string RecruitingMomsServiceQuizAnswerSaveQueueName = "RecruitingMomsServiceQuizAnswerSaveQueue";

        #endregion

        #region ScheduleEvent

        public const string RecruitingMomsServiceForecastQueueName = "RecruitingMomsServiceForecastQueue";
        public const string EventScheduleServiceQueueName = "RecruitingMomsEventScheduleServiceQueue";

        #endregion

        #region Article

        public const string RecruitingMomsServiceItemMaster = "RecruitingMomsServiceItemMaster";

        #endregion

        #region MutationLog

        public const string RecruitingMomsServiceDataMutationQueueName = "RecruitingMomsServiceDataMutationQueue";

        #endregion

        #region Gql

        public const string GraphQlServiceExchangeName = "GraphQlServiceExchange";
        public const string RecruitingMomsServiceGraphQLQueueName = "RecruitingMomsServiceGraphQLQueue";

        #endregion

        #region GenericEvent or UAM

        public const string GenericEventExchangeName = "Selise.Ecap.Events:GenericEvent";
        public const string RecruitingMomsServiceGenericEventQueueName = "RecruitingMomsServiceGenericEventQueue";

        #endregion

        #region FVS

        public const string EcapFormValidationServiceSubmissionEventExchangeName =
            "Selise.Ecap.FormValidationService:SubmissionEvent";

        public const string RecruitingMomsServiceFormValidationSubmissionEventQueueName =
            "RecruitingMomsServiceFormValidationSubmissionEventQueue";

        #endregion

        #region Storage

        public const string RecruitingMomsServiceStorageQueueName = "RecruitingMomsServiceStorageQueue";

        #endregion

        #region HR

        public const string EmployeeServiceQueueName = "RecruitingMomsEmployeeServiceQueue";

        #endregion

        #region Others

        public const string EcapRecruitingMomsServiceQueueName = "EcapRecruitingMomsServiceQueue";
        public const string RecruitingMomsCommandQueueName = "RecruitingMomsCommandQueue";
        public const string RecruitingMomsServiceSchedulerEventQueueName = "RecruitingMomsServiceSchedulerEventQueue";
        public const string RecruitingMomsEtlServiceQueueName = "RecruitingMomsEtlServiceQueue";
        public const string StorageGetFileByIdQueryParam = "FileId";

        public const string RecruitingMomsServiceDocumentFileGenerationQueue1Name =
            "Selise.Ecap.RecruitingMoms.DocumentFileGenerationQueue1";

        public const string RecruitingMomsServiceDocumentFileGenerationQueue2Name =
            "Selise.Ecap.RecruitingMoms.DocumentFileGenerationQueue2";

        public const string GenerateRenderedFilesSuccessExchangeName =
            "Selise.Ecap.TemplateEngine.Events:GenerateRenderedFilesSuccessEvent";

        public const string GenerateRenderedFilesFailureExchangeName =
            "Selise.Ecap.TemplateEngine.Events:GenerateRenderedFilesFailureEvent";

        public const string PdfsFromHtmlCreatedBulkEventExchangeName =
            "Selise.Ecap.PdfGenerator.Events.Dtos:PdfsFromHtmlCreatedBulkEvent";

        #endregion

}