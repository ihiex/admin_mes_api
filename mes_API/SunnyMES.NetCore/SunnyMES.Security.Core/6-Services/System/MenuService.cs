using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    /// <summary>
    /// �˵�
    /// </summary>
    public class MenuService: BaseService<Menu, MenuOutputDto, string>, IMenuService
    {
        private readonly IMenuRepository _MenuRepository;
        private readonly IUserRepository userRepository;
        private readonly ISystemTypeRepository systemTypeRepository;
        private readonly IRoleAuthorizeRepository roleAuthorizeRepository;
        private readonly ILogService _logService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="_userRepository"></param>
        /// <param name="_roleAuthorizeRepository"></param>
        /// <param name="_systemTypeRepository"></param>
        /// <param name="logService"></param>
        public MenuService(IMenuRepository repository,IUserRepository _userRepository, IRoleAuthorizeRepository _roleAuthorizeRepository, ISystemTypeRepository _systemTypeRepository, ILogService logService) : base(repository)
        {
            _MenuRepository = repository;
            userRepository = _userRepository;
            roleAuthorizeRepository = _roleAuthorizeRepository;
            systemTypeRepository = _systemTypeRepository;
            _logService = logService;
        }

        /// <summary>
        /// �����û���ȡ���ܲ˵�
        /// </summary>
        /// <param name="userId">�û�ID</param>
        /// <returns></returns>
        public List<Menu> GetMenuByUser(string userId)
        {
            List<Menu> result = new List<Menu>();
            List<Menu> allMenuls = new List<Menu>();
            List<Menu> subMenuls = new List<Menu>();
            string where = string.Format("Layers=1");
            IEnumerable<Menu> allMenus = _MenuRepository.GetAllByIsNotDeleteAndEnabledMark();
            allMenuls = allMenus.ToList();
            if (userId == string.Empty) //��������Ա
            {
                return allMenuls;
            }
            var user = userRepository.Get(userId);
            if (user == null)
                return result;
            var userRoles = user.RoleId;
            where = string.Format("ItemType = 1 and ObjectType = 1 and ObjectId='{0}'",userRoles);
            var Menus = roleAuthorizeRepository.GetListWhere(where);
            foreach (RoleAuthorize item in Menus)
            {
                Menu MenuEntity = allMenuls.Find(t => t.Id == item.ItemId);
                if (MenuEntity != null)
                {
                    result.Add(MenuEntity);
                }
            }
            return result.OrderBy(t => t.SortCode).ToList();
        }


        /// <summary>
        /// ��ȡ���ܲ˵�������Vue �����б�
        /// </summary>
        /// <param name="systemTypeId">��ϵͳId</param>
        /// <returns></returns>
        public async Task<List<MenuTreeTableOutputDto>> GetAllMenuTreeTable(string systemTypeId)
        {
            string where = "1=1";
            List<MenuTreeTableOutputDto> reslist = new List<MenuTreeTableOutputDto>();
            if (!string.IsNullOrEmpty(systemTypeId))
            {
                IEnumerable<Menu> elist = await _MenuRepository.GetListWhereAsync("SystemTypeId='" + systemTypeId + "'");
                List<Menu> list = elist.OrderBy(t => t.SortCode).ToList();
                List<Menu> oneMenuList = list.FindAll(t => t.ParentId == "");
                foreach (Menu item in oneMenuList)
                {
                    MenuTreeTableOutputDto menuTreeTableOutputDto = new MenuTreeTableOutputDto();
                    menuTreeTableOutputDto = item.MapTo<MenuTreeTableOutputDto>();
                    menuTreeTableOutputDto.Children = GetSubMenus(list, item.Id).ToList<MenuTreeTableOutputDto>();
                    reslist.Add(menuTreeTableOutputDto);
                }

            }
            else
            {
                IEnumerable<SystemType> listSystemType = await systemTypeRepository.GetListWhereAsync(where);

                foreach (SystemType systemType in listSystemType)
                {
                    MenuTreeTableOutputDto menuTreeTableOutputDto = new MenuTreeTableOutputDto();
                    menuTreeTableOutputDto.Id = systemType.Id;
                    menuTreeTableOutputDto.FullName = systemType.FullName;
                    menuTreeTableOutputDto.EnCode = systemType.EnCode;
                    menuTreeTableOutputDto.UrlAddress = systemType.Url;
                    menuTreeTableOutputDto.EnabledMark = systemType.EnabledMark;

                    menuTreeTableOutputDto.SystemTag = true;

                    IEnumerable<Menu> elist = await _MenuRepository.GetListWhereAsync("SystemTypeId='" + systemType.Id + "'");
                    if (elist.Count() > 0)
                    {
                        List<Menu> list = elist.OrderBy(t => t.SortCode).ToList();
                        menuTreeTableOutputDto.Children = GetSubMenus(list, "").ToList<MenuTreeTableOutputDto>();
                    }
                    reslist.Add(menuTreeTableOutputDto);
                }
            }
            return reslist;
        }


        /// <summary>
        /// ��ȡ�Ӳ˵����ݹ����
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parentId">����Id</param>
        /// <returns></returns>
        private List<MenuTreeTableOutputDto> GetSubMenus(List<Menu> data, string parentId)
        {
            List<MenuTreeTableOutputDto> list = new List<MenuTreeTableOutputDto>();
            MenuTreeTableOutputDto menuTreeTableOutputDto = new MenuTreeTableOutputDto();
            var ChilList = data.FindAll(t => t.ParentId == parentId);
            foreach (Menu entity in ChilList)
            {
                menuTreeTableOutputDto = entity.MapTo<MenuTreeTableOutputDto>();
                menuTreeTableOutputDto.Children = GetSubMenus(data, entity.Id).OrderBy(t => t.SortCode).MapTo<MenuTreeTableOutputDto>();
                list.Add(menuTreeTableOutputDto);
            }
            return list;
        }

        /// <summary>
        /// ���ݽ�ɫID�ַ��������ŷֿ�)��ϵͳ����ID����ȡ��Ӧ�Ĳ��������б�
        /// </summary>
        /// <param name="roleIds">��ɫID</param>
        /// <param name="typeID">ϵͳ����ID</param>
        /// <param name="UserID">UserID</param>
        /// <param name="isMenu">�Ƿ��ǲ˵�</param>
        /// <returns></returns>
        public List<Menu> GetFunctions(string roleIds, string typeID, string UserID, bool isMenu=false)
        {
            return _MenuRepository.GetFunctions(roleIds, typeID, UserID,isMenu).ToList();
        }


        /// <summary>
        /// ����ϵͳ����ID����ȡ��Ӧ�Ĳ��������б�
        /// </summary>
        /// <param name="typeID">ϵͳ����ID</param>
        /// <returns></returns>
        public List<Menu> GetFunctions(string typeID)
        {
            return _MenuRepository.GetFunctions(typeID).ToList();
        }


        /// <summary>
        /// ���ݸ������ܱ����ѯ�����Ӽ����ܣ���Ҫ����ҳ�������ťȨ��
        /// </summary>
        /// <param name="enCode">�˵����ܱ���</param>
        /// <returns></returns>
        public async Task<IEnumerable<MenuOutputDto>> GetListByParentEnCode(string enCode)
        {
            string where = string.Format("EnCode='{0}'", enCode);
            Menu function = await repository.GetWhereAsync(where);
            where = string.Format("ParentId='{0}'", function.ParentId);
            IEnumerable<Menu> list = await repository.GetAllByIsNotEnabledMarkAsync(where);
            return list.MapTo<MenuOutputDto>().ToList();
        }
        /// <summary>
        /// ����������ɾ��
        /// </summary>
        /// <param name="idsInfo">����Id����</param>
        /// <param name="trans">�������</param>
        /// <returns></returns>
        public CommonResult DeleteBatchWhere(DeletesInputDto idsInfo, IDbTransaction trans = null)
        {
            CommonResult result = new CommonResult();
            string where = string.Empty;
            for (int i = 0; i < idsInfo.Ids.Length; i++)
            {
                if (idsInfo.Ids[0] != null)
                {
                    where = string.Format("ParentId='{0}'", idsInfo.Ids[0]);
                    IEnumerable<Menu> list = _MenuRepository.GetListWhere(where);
                    if (list.Count()>0)
                    {
                        result.ResultMsg = "���ܴ����Ӽ����ݣ�����ɾ��";
                        return result;
                    }
                }
            }
            where = "id in ('" + idsInfo.Ids.Join(",").Trim(',').Replace(",", "','") + "')";
            bool bl = repository.DeleteBatchWhere(where);
            if (bl)
            {
                result.ResultCode ="0";
            }
            return result;
        }

        /// <summary>
        /// ����������ɾ��
        /// </summary>
        /// <param name="idsInfo">����Id����</param>
        /// <param name="trans">�������</param>
        /// <returns></returns>
        public async Task<CommonResult> DeleteBatchWhereAsync(DeletesInputDto idsInfo,IDbTransaction trans = null)
        {
            CommonResult result = new CommonResult();
            string where =string.Empty;
            for (int i =0;i< idsInfo.Ids.Length;i++)
            {
                if (idsInfo.Ids[0].ToString().Length>0)
                {
                    where = string.Format("ParentId='{0}'", idsInfo.Ids[0]);
                    IEnumerable<Menu> list = _MenuRepository.GetListWhere(where);
                    if (list.Count()>0)
                    {
                        result.ResultMsg = "���ܴ����Ӽ����ݣ�����ɾ��";
                        return result;
                    }
                }
            }
            where = "id in ('" + idsInfo.Ids.Join(",").Trim(',').Replace(",", "','") + "')";
            bool bl = await repository.DeleteBatchWhereAsync(where);
            if (bl)
            {
                result.ResultCode = "0";
            }
            return result;
        }
    }
}