namespace Showaspnetcore.Data
{
    public partial class RolePermission
    {
        public string RoleId { get; set; }
        public int PermissionId { get; set; }

        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
