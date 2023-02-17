using Blog.Domain.Models;

namespace Blog.Admin.Areas.Account.VIewModels.Settings;

public class EditorAccountsViewModel
{
    public IList<UserInfoDto> Accounts { get; set; } = new List<UserInfoDto>();
}