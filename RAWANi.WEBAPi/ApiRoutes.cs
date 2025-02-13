namespace RAWANi.WEBAPi
{
    public static class ApiRoutes
    {
        /// <summary>
        /// The base route for all API endpoints.
        /// </summary>
        public const string BaseRoute = "api/v{version:apiVersion}/[controller]";

        /// <summary>
        /// Routes related to countries.
        /// </summary>
        public static class Countries
        {
            /// <summary>
            /// Route template for accessing a specific country by ID.
            /// </summary>
            public const string GetById = "{countryId:int}";
        }

        public static class AdminRoutes
        {
            //Roles
            public const string RolesRoute = "roles";
            public const string AssignRole = "roles/assign";
            public const string RemoveRole = "roles/remove";

            //User
            public const string UserRouts = "users";
            public const string UserIdRoute = "users/{userId}";
            public const string UsernameRoute = "users/username";
        }










        /// <summary>
        /// Routes related to persons.
        /// </summary>
        public static class UserProfileRoutes
        {
            public const string UserProfileIdRoute = "user-profile-id/{userProfileId}";
            public const string UserIdRoute = "user-id/{userId}";
            public const string CurrentUserProfile = "current-user-profile";
           
        }

        public static class Roles
        {
            public const string RoleIDRoute = "{roleID}";
            public const string AssignRole = "assign";
            public const string RemoveRole = "unassign";
        }

        /// <summary>
        /// Routes related to user profiles.
        /// </summary>
        public static class UserRoutes
        {
            /// <summary>
            /// Route template for accessing a specific user profile by ID.
            /// </summary>

            public const string UserGuidRout = "user-id/{userGuid}";
            public const string PersonGuidRout = "person-id/{personGuid}";
            public const string UserEmail = "by-email/{email}";
            public const string UserFirstAndLastNamesUpdate = "update-names/{userGuid}";
            public const string AssignRoleToUser = "{userId}/roles";
            

        }

        /// <summary>
        /// Routes related to posts.
        /// </summary>
        public static class Posts
        {
            /// <summary>
            /// Route template for accessing a specific post by ID.
            /// </summary>
            public const string PostIdRoute = "{postId}";
            public const string UpdatePostImage = "update-post-image/{postId}";

            /// <summary>
            /// Route template for accessing comments for a specific post.
            /// </summary>
            public const string PostCommentsRoute = "{postId}/comments";
            public const string PostWithCommentsRoute = "{postId}/post-with-comments";
            public const string CommentIDRoute = "comments/{CommentId}";

            /// <summary>
            /// Route template for accessing a specific comment by ID within a post.
            /// </summary>
            public const string PostOneCommentIdRoute = "{postId}/comments/{commentId}";
        }

        /// <summary>
        /// Routes related to identity management.
        /// </summary>
        public static class AuthRouts
        {
            /// <summary>
            /// Route template for user login.
            /// </summary>
            public const string Login = "login";
            public const string Logout = "logout";
            /// <summary>
            /// Route template for user registration.
            /// </summary>
            public const string Registration = "registration";
            public const string RefreshToken = "refresh-token";
            public const string Roles = "roles";
            public const string UpdateRole = "roles/{roleId}";
            public const string DeleteRole = "roles/{roleId}";
            public const string ForgotPassword = "forgot-password";
            public const string ResetPassword = "reset-password";
            public const string ConfirmEmail = "confirm-email";
        }

        public static class Protected
        {
            public const string GetProtected = "protected";
        }
    }
}
