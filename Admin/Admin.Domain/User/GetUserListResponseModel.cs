namespace Admin.Domain.User
{
    public class GetUserListResponseModel
    {
        public string UserId { get; set;}
        public string UserName { get; set;}
    }
    public class AllUserListResponseModel
    {
        public IEnumerable<GetUserListResponseModel>? UserList { get; set; }
    }
}
