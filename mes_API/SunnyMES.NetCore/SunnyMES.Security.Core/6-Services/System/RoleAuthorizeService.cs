using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleAuthorizeService: BaseService<RoleAuthorize, RoleAuthorizeOutputDto, string>, IRoleAuthorizeService
    {
        private readonly IRoleAuthorizeRepository _repository;

        private readonly IMenuRepository menuRepository;
        private readonly ISystemTypeRepository systemTypeRepository;
        private readonly ILogService _logService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="_menuRepository"></param>
        /// <param name="_systemTypeRepository"></param>
        /// <param name="logService"></param>
        public RoleAuthorizeService(IRoleAuthorizeRepository repository,  IMenuRepository _menuRepository, ISystemTypeRepository _systemTypeRepository, ILogService logService) : base(repository)
        {
            _repository = repository;
            menuRepository = _menuRepository;
            systemTypeRepository = _systemTypeRepository;
            _logService = logService;
        }


        /// <summary>
        /// ���ݽ�ɫ����Ŀ���Ͳ�ѯȨ��
        /// </summary>
        /// <param name="roleIds">��ɫId</param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public IEnumerable<RoleAuthorize> GetListRoleAuthorizeByRoleId(string roleIds, string itemType)
        {
            IEnumerable<RoleAuthorize> list = _repository.GetListWhere(string.Format("ItemType in({0}) and ObjectId in ({1}) and ObjectType=1", itemType, roleIds));
            return list;
        }


        /// <summary>
        /// ��ȡ���ܲ˵�������Vue Tree����
        /// </summary>
        /// <returns></returns>
        public async Task<List<ModuleFunctionOutputDto>> GetAllFunctionTree()
        {
            string where = "1=1";
            List<ModuleFunctionOutputDto> reslist = new List<ModuleFunctionOutputDto>();
            IEnumerable<SystemType> listSystemType = await systemTypeRepository.GetListWhereAsync(where);
            foreach (SystemType systemType in listSystemType)
            {
                ModuleFunctionOutputDto menuTreeTableOutputDto = new ModuleFunctionOutputDto();
                menuTreeTableOutputDto.Id = systemType.Id;
                menuTreeTableOutputDto.FullName = systemType.FullName;
                menuTreeTableOutputDto.FunctionTag =0;
                menuTreeTableOutputDto.IsShow = true;

                IEnumerable<Menu> elist = await menuRepository.GetListWhereAsync("SystemTypeId='" + systemType.Id + "'");
                if (elist.Count() > 0)
                {
                    List<Menu> list = elist.OrderBy(t => t.SortCode).ToList();
                    menuTreeTableOutputDto.Children = GetSubMenus(list, "").ToList<ModuleFunctionOutputDto>();
                }
                reslist.Add(menuTreeTableOutputDto);
            }
            return reslist;
        }


        /// <summary>
        /// ��ȡ�Ӳ˵����ݹ����
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parentId">����Id</param>
        /// <returns></returns>
        private List<ModuleFunctionOutputDto> GetSubMenus(List<Menu> data, string parentId)
        {
            List<ModuleFunctionOutputDto> list = new List<ModuleFunctionOutputDto>();
            var ChilList = data.FindAll(t => t.ParentId == parentId);
            foreach (Menu entity in ChilList)
            {
                ModuleFunctionOutputDto menuTreeTableOutputDto = new ModuleFunctionOutputDto();
                menuTreeTableOutputDto.Id= entity.Id;
                menuTreeTableOutputDto.FullName = entity.FullName;
                menuTreeTableOutputDto.IsShow = false;
                if (entity.MenuType == "F")
                {
                    menuTreeTableOutputDto.FunctionTag = 2;
                }
                else
                {
                    menuTreeTableOutputDto.FunctionTag = 1;
                }
                    menuTreeTableOutputDto.Children = GetSubMenus(data, entity.Id).MapTo<ModuleFunctionOutputDto>();
                list.Add(menuTreeTableOutputDto);
            }
            return list;
        }
        /// <summary>
        /// �����ɫ��Ȩ
        /// </summary>
        /// <param name="roleId">��ɫId</param>
        /// <param name="roleAuthorizesList">��ɫ����ģ��</param>
        /// <param name="roleDataList">��ɫ�ɷ�������</param>
        /// <param name="trans"></param>
        /// <returns>ִ�гɹ�����<c>true</c>������Ϊ<c>false</c>��</returns>
        public async Task<bool> SaveRoleAuthorize(string roleId,List<RoleAuthorize> roleAuthorizesList, List<RoleData> roleDataList,
           IDbTransaction trans = null)
        {
           return await  _repository.SaveRoleAuthorize(roleId,roleAuthorizesList, roleDataList);
        }
    }
}