using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Models
{
    public class ErrorMessage
    {
       
        public const string InvalidClaim = ": Invalid or missing claim.";

        public const string ResourceRetrievalFailed = "Failed to retrieve resources.";

        public const string NotFound = ": Resource Not Found.";
        public const string OperationCanceled = ": Operation canceled due to client request termination.";
        public const string DatabaseError = ": Database error; Failed to procces data.";
        public const string UnknownError = ": An unexpected error occurred.";
        public const string ErrorMappingData = ": Error occurred while mapping resources.";
        public const string CreationFailed = ": Failed to create Resource. ";
        public const string EmailConflict = ": Email conflict.";
        public const string GOVIDCONFLICT = ": Government id conflict";
        public const string CONFLICTERROR = ": Conflict error occurred.";
        public const string ResourceAlreadyExists = "This resource already exists in the system.";
       



        //public const string UnexpectedError = "An unexpected error occurred";
        //public const string UserNotFound = "The specified user could not be found.";
        //public const string ResourceNotFound = "The requested resource was not found in the database.";
        //public const string UserProfileNotFound = "The specified user profile could not be found.";
        //public const string IncorrectPassword = "The password entered is incorrect.";
        //public const string EmailInUse = "The provided email address is already in use.";
        //public const string CreationFailed = "Unable to create";
        //public const string DeletionFailed = "Failed to delete the resource.";
        //public const string SaveFailed = "Failed to persist the data into the database.";
        //public const string TransactionRollback = "Error occurred, Transaction rolled back";
        //public const string Unauthorized = "You are not authorized to modify this resource.";

    }
}
