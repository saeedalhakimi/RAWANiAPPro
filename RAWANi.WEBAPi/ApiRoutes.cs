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

        /// <summary>
        /// Routes related to persons.
        /// </summary>
        public static class Persons
        {
            /// <summary>
            /// Route template for accessing a specific person by GUID.
            /// </summary>
            public const string GetByGuid = "{guid}";

            /// <summary>
            /// Route template for accessing a specific person by person number.
            /// </summary>
            public const string GetByPersonNumber = "personnumber/{personNumber}";
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
            public const string GetById = "{id}";

            /// <summary>
            /// Route template for accessing comments for a specific post.
            /// </summary>
            public const string GetCommentsByPostId = "{postId}/comments";

            /// <summary>
            /// Route template for accessing a specific comment by ID within a post.
            /// </summary>
            public const string GetCommentById = "{postId}/comments/{commentId}";
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
        }

        public static class Protected
        {
            public const string GetProtected = "protected";
        }
    }
}
