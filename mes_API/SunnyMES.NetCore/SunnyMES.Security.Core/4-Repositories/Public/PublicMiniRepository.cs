using Dapper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.Dtos.Models;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Security.Models;
using SunnyMES.Security.ToolExtensions;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security._1_Models.MES.Query.ShipMent;
using SunnyMES.Security._1_Models.MES;

namespace SunnyMES.Security.Repositories;

public class PublicMiniRepository : PublicRepository
{
    public PublicMiniRepository()
    {
        
    }
    public PublicMiniRepository(IDbContextCore dbContext, int I_Language):base(dbContext, I_Language)
    {

    }
    public async Task<string> uspKitBoxFGSNCheck(string UPCSN,string FGSN,string xmlPOID,string xmlPartID,string xmlStation,string xmlExtraData) => await base.uspCallProcedureAsync("uspKitBoxFGSNCheck", UPCSN, xmlPOID, xmlPartID, xmlStation, xmlExtraData, FGSN);
    public async Task<List<mesStationType>> MesGetStationTypeAsync(string ID)
    {
        string S_Sql = "select * from mesStationType";
        if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

        var v_Query = await DapperConn.QueryAsync<mesStationType>(S_Sql, null, null, I_DBTimeout, null);
        return v_Query.AsList();
    }

	/// <summary>
	/// 通过ID获取状态描述
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
    public async Task<mesUnitState> getMesUnitStateDescAsync(string ID) =>  await DapperConn.QueryFirstOrDefaultAsync<mesUnitState>($"SELECT * FROM dbo.mesUnitState WHERE ID = {ID}", null, null, I_DBTimeout, null);

    public async Task<KitBoxCheckOutput> uspKitBoxCheckAsync(string S_FormatName, string xmlProdOrder, string xmlPart, string xmlStation, string xmlExtraData, string strSNbuf)
    {
        string S_Sql = @$"
        BEGIN
	    declare	@PackageId	int,
			    @kitPartID	int,
			    @idoc		int,
			    @txtPart    varchar(8000),
			    @partId		int,
			    @prodId		int,
			    @stationId  int,
			    @employeeId int,
			    @isMixedPO	int,
			    @isMixedPN	int,
			    @isScanOnlyFG int,
			    @MPN		varchar(200),
			    @BoxMPN		varchar(200),
			    @UPC		varchar(200),
			    @BoxUPC		varchar(200),
			    @BoxSN		varchar(200),
			    @GTIN		varchar(200),
			    @PalletSN	VARCHAR(128),
			    @PalletStatusID INT,
			    @strSNFormat      nvarchar(200) = '{S_FormatName}',		
			    @xmlProdOrder     nvarchar(2000) = {(string.IsNullOrEmpty(xmlProdOrder) ? "null" : $"'{xmlProdOrder}'")}, 
			    @xmlPart          nvarchar(2000) = {(string.IsNullOrEmpty(xmlPart) ? "null" : $"'{xmlPart}'")},  
			    @xmlStation       nvarchar(2000) = {(string.IsNullOrEmpty(xmlStation) ? "null" : $"'{xmlStation}'")}, 
			    @xmlExtraData     nvarchar(2000) = {(string.IsNullOrEmpty(xmlExtraData) ? "null" : $"'{xmlExtraData}'")},
			    @strSNbuf         nvarchar(200) = '{strSNbuf}',			--传入参数类型BOX/SN
			    @strOutput        nvarchar(200)

		BEGIN TRY
		    --运行时间记录
		    --DECLARE @index int
		    --INSERT INTO Sys_LogRecord(SN,ProName,StartTime)
		    --Values (@strSNFormat,'uspKitBoxCheck',GETDATE())
		    --SET @index = SCOPE_IDENTITY()

		    SET @strOutput='1'

		    --读取xmlPart参数值
		    exec sp_xml_preparedocument @idoc output, @xmlPart
		    SELECT @partId=PartId
		    FROM   OPENXML (@idoc,			'/Part',2)
				    WITH (PartId  int		'@PartID')   
		    exec sp_xml_removedocument @idoc

		    IF isnull(@partId,'') = ''
		    begin
			    SET @strOutput='20077'
			    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
			    RETURN
		    end

		    --读取ProdOrder参数值
		    exec sp_xml_preparedocument @idoc output, @xmlProdOrder
		    SELECT @prodId=ProdId
		    FROM   OPENXML (@idoc,			'/ProdOrder',2)
				    WITH (ProdId  int		'@ProdOrderID')   
		    exec sp_xml_removedocument @idoc

		    IF isnull(@prodId,'') = ''
		    begin
			    SET @strOutput= '20077'
			    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
			    RETURN
		    end

		    --配置参数获取
		    SELECT @UPC = A.Content FROM mesProductionOrderDetail A INNER JOIN luProductionOrderDetailDef B
			    ON A.ProductionOrderDetailDefID=B.ID AND B.Description='UPC' AND A.ProductionOrderID=@prodId
		    IF ISNULL(@UPC,'')=''
		    BEGIN
			    SET @strOutput= '20071'
			    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
			    RETURN
		    END

		    SELECT @MPN = A.Content FROM mesProductionOrderDetail A INNER JOIN luProductionOrderDetailDef B
			    ON A.ProductionOrderDetailDefID=B.ID AND B.Description='MPN' AND A.ProductionOrderID=@prodId
		    IF ISNULL(@MPN,'')=''
		    BEGIN
			    SET @strOutput= '20072'
			    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
			    RETURN
		    END

		    IF @strSNbuf='BOX'
		    BEGIN
			    --IF LEN(@strSNFormat)>20 
			    --BEGIN
				   -- --箱号格式校验
				   -- IF CHARINDEX('GTIN',@strSNFormat)=0 OR CHARINDEX('SSCC',@strSNFormat)=0 
					  --  OR CHARINDEX('MPN',@strSNFormat)=0 
				   -- BEGIN
					  --  SET @strOutput= '20078'
					  --  SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					  --  RETURN
				   -- END

				   -- BEGIN TRY
					  --  SET @BoxSN   = '00'+DBO.ufnGetBoxSubString(@strSNFormat,'SSCC')
					  --  SET @GTIN    =  DBO.ufnGetBoxSubString(@strSNFormat,'GTIN')
					  --  SET @BoxMPN  =  DBO.ufnGetBoxSubString(@strSNFormat,'MPN')
					  --  SET @BoxUPC  =  SUBSTRING(@GTIN,3,LEN(@GTIN)-2)
					  --  END TRY
				   -- BEGIN CATCH
					  --  SET @strOutput= '20079'
					  --  SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					  --  RETURN
				   -- END CATCH

			    --  --校验Box条码长度
				   --   if len(@BoxSN)<>20
				   --   BEGIN
					  --  SET @strOutput= '20078'
					  --  SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					  --  RETURN
				   --   END
				   -- --校验参数
				   -- IF @BoxMPN<>@MPN
				   -- BEGIN
					  --  SET @strOutput= '20080'
					  --  SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					  --  RETURN
				   -- END
				   -- IF @BoxUPC<>@UPC
				   -- BEGIN
					  --  SET @strOutput= '20081'
					  --  SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					  --  RETURN
				   -- END
			    --END
			    --ELSE
			    --BEGIN
				   -- SET @BoxSN=@strSNFormat
			    --END
				--DECLARE @strOutput1 NVARCHAR(200)
				--, @BoxSnOutput NVARCHAR(200);
                EXEC dbo.UDPGetKitBoxSN @strSNFormat=@strSNFormat, -- nvarchar(200)
				    @xmlProdOrder=@xmlProdOrder, -- text
				    @xmlPart=@xmlPart, -- text
				    @xmlStation='', -- text
				    @xmlExtraData='', -- text
				    @strSNbuf=N'', -- nvarchar(200)
				    @strOutput=@strOutput OUTPUT, -- nvarchar(200)
				    @BoxSnOutput=@BoxSN OUTPUT -- nvarchar(200)
				
				IF(@strOutput <> '1')
				BEGIN
				     SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					 RETURN
				END		

				if(ISNULL(@BoxSN,'') = '')
				begin
					SET @strOutput= '20078'
					SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					RETURN
				end	

			    --校验条码长度
			    --if len(@BoxSN)<>101
			    --BEGIN
			    --	SET @strOutput= '20078'
			    --	RETURN
			    --END
			    IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@BoxSN AND STATUSID=1)
			    BEGIN
				    SET @strOutput= '20119'
					    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					    RETURN
			    END

			    --判断箱号是否存在（不存在Insert）
			    IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@BoxSN)
			    BEGIN
				    --读取Station参数
				    exec sp_xml_preparedocument @idoc output, @xmlStation
				    SELECT @stationId=StationId
				    FROM   OPENXML (@idoc, '/Station',2)
						    WITH (StationId		  int			'@StationId')   
				    exec sp_xml_removedocument @idoc

				    --读取@employeeId参数
				    exec sp_xml_preparedocument @idoc output, @xmlExtraData
				    SELECT @employeeId=EmployeeId
				    FROM   OPENXML (@idoc, '/ExtraData',2)
						    WITH (EmployeeId	  int			'@EmployeeId')   
				    exec sp_xml_removedocument @idoc

				    INSERT INTO mesPackage(ID,SerialNumber,StationID,EmployeeID,CreationTime,StatusID,LastUpdate,Stage,CurrProductionOrderID,CurrPartID)
				    VALUES
				    ((SELECT isnull(MAX(ID),0) FROM mesPackage)+1,@BoxSN,@stationId,@employeeId,GETDATE(),0,GETDATE(),1,@prodId,@partId)
			    END
			    ELSE
			    BEGIN
				    --读取ProdOrder参数 20201008 Add 
				    exec sp_xml_preparedocument @idoc output, @xmlProdOrder
				    SELECT @prodId=prodId
				    FROM   OPENXML (@idoc, '/ProdOrder',2)
						    WITH (prodId		  int			'@ProdOrderID')   
				    exec sp_xml_removedocument @idoc
				    --判断箱号是否和选择工单一致
				    IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @BoxSN AND CurrProductionOrderID = @prodId)
				    BEGIN
					    SET @strOutput= '20133'
					    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					    RETURN
				    END
			    END
			    --SELECT @BoxSN AS BoxSN
			    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 

		    END
		    ELSE IF @strSNbuf='SN'
		    BEGIN
			    SELECT @isScanOnlyFG=ISNULL(A.Content,0) FROM mesProductionOrderDetail A 
			    INNER JOIN luProductionOrderDetailDef B ON A.ProductionOrderDetailDefID=B.ID
			    WHERE A.ProductionOrderID=@prodId AND B.Description='MultipackScanOnlyFGSN'

			    --获取配置是否可以混料号/工单
			    SELECT @isMixedPO = A.Content FROM mesProductionOrderDetail A INNER JOIN luProductionOrderDetailDef B
				    ON A.ProductionOrderDetailDefID=B.ID AND B.Description='IsMixedPO' AND A.ProductionOrderID=@prodId

			    SELECT @isMixedPN = A.Content FROM mesProductionOrderDetail A INNER JOIN luProductionOrderDetailDef B
				    ON A.ProductionOrderDetailDefID=B.ID AND B.Description='IsMixedPN' AND A.ProductionOrderID=@prodId

			    IF @isScanOnlyFG = 1
			    BEGIN
				    DECLARE @UnitID	int
				    SELECT @UnitID=UnitID FROM mesSerialNumber WHERE Value=@strSNFormat
				    IF isnull(@UnitID,'')=''
				    BEGIN
					    SET @strOutput= '20012'
					    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					    RETURN
				    END

				    IF NOT EXISTS(SELECT 1 FROM mesUnit A WHERE A.ID=@UnitID AND A.StatusID=1)
				    BEGIN
					    SET @strOutput= '20036'
					    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					    RETURN
				    END

				    --不能混料号进行校验
				    IF isnull(@isMixedPN,0)<>1
				    BEGIN
					    IF NOT EXISTS( SELECT 1 FROM mesUnitDetail A INNER JOIN mesUnit B ON A.UnitID=B.ID
						    WHERE B.ID=@UnitID AND B.PartID=@partId)
					    BEGIN
						    SET @strOutput= '20083'
						    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
						    RETURN
					    END
				    END
				    --不能混工单进行校验
				    IF isnull(@isMixedPO,0)<>1
				    BEGIN
					    IF NOT EXISTS(SELECT 1 FROM mesUnitDetail A INNER JOIN mesUnit B ON A.UnitID=B.ID 
					    WHERE  B.ID=@UnitID AND B.ProductionOrderID=@prodId)
					    BEGIN
						    SET @strOutput= '20084'
						    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
						    RETURN
					    END
				    END

				    IF EXISTS(SELECT 1 FROM mesUnitDetail WHERE UnitID=@UnitID and ISNULL(InmostPackageID,'')<>'')
				    BEGIN
					    SELECT TOP 1 @BoxSN=A.SerialNumber,@PalletStatusID=A.StatusID FROM mesPackage A 
						    INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
						    INNER JOIN mesSerialNumber C ON B.UnitID=C.UnitID 
					    WHERE C.Value=@strSNFormat
					    IF(isnull(@PalletStatusID,'') in (0,1))
					    BEGIN
						    SET @strOutput='0'
						    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
						    RETURN
					    END
					    ELSE
					    BEGIN
						    SET @strOutput= '20082'
						    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
						    RETURN
					    END
				    END
			    END
			    ELSE
			    BEGIN
				    IF not exists(SELECT 1 FROM mesSerialNumber WHERE Value=@strSNFormat)
				    BEGIN
					    SET @strOutput= '20012'
					    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					    RETURN
				    END
				    IF NOT EXISTS(SELECT 1 FROM mesUnitDetail WHERE KitSerialNumber=@strSNFormat)
				    BEGIN
					    SET @strOutput= '20189'
					    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					    RETURN
				    END

				    IF NOT EXISTS(select 1 from mesUnit A inner join mesUnitDetail b on a.ID=b.UnitID
	                                        where b.KitSerialNumber=@strSNFormat AND A.StatusID=1)
				    BEGIN
					    SET @strOutput= '20036'
					    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
					    RETURN
				    END

				    --不能混料号进行校验
				    IF isnull(@isMixedPN,0)<>1
				    BEGIN
					    IF NOT EXISTS( SELECT 1 FROM mesUnitDetail A INNER JOIN mesUnit B ON A.UnitID=B.ID
						    WHERE KitSerialNumber=@strSNFormat AND B.PartID=@partId)
					    BEGIN
						    SET @strOutput= '20083'
						    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
						    RETURN
					    END
				    END
				    --不能混工单进行校验
				    IF isnull(@isMixedPO,0)<>1
				    BEGIN
					    IF NOT EXISTS(SELECT 1 FROM mesUnitDetail A INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE A.KitSerialNumber=@strSNFormat
						    AND B.ProductionOrderID=@prodId)
					    BEGIN
						    SET @strOutput= '20084'
						    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
						    RETURN
					    END
				    END

				    IF EXISTS(SELECT 1 FROM mesUnitDetail WHERE KitSerialNumber=@strSNFormat and ISNULL(InmostPackageID,'')<>'')
				    BEGIN
					    SELECT TOP 1 @BoxSN=A.SerialNumber,@PalletStatusID=A.StatusID FROM mesPackage A 
						    INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
					    WHERE B.KitSerialNumber=@strSNFormat
					    IF(isnull(@PalletStatusID,'')=0)
					    BEGIN
						    SET @strOutput='0'
						    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
						    RETURN
					    END
					    ELSE
					    BEGIN
						    SET @strOutput= '20082'
						    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
						    RETURN
					    END
				    END
			    END
		    END
		    ELSE
		    BEGIN
			    SET @strOutput= '20085'
			    SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 
			    RETURN
		    END
			SELECT @BoxSN AS BoxSN,   @strOutput AS strOutput 

		    ----运行时间记录
		    --update Sys_LogRecord set EndTime=GETDATE() where ID=@index
	    END TRY
	    BEGIN CATCH
            SELECT @BoxSN AS BoxSN,   ERROR_MESSAGE() AS strOutput
	    END CATCH
    END
";

        var v_Query = await DapperConn.QueryFirstOrDefaultAsync<KitBoxCheckOutput>(S_Sql, null, null, I_DBTimeout, null);
        return v_Query;
    }

    public async Task<List<CartonBoxConfirmed>> GetBoxPackageDataAsync(string SN)
    {
        string sql =
            @$"SELECT ROW_NUMBER() OVER(ORDER BY A.ID) SEQNO, KitSerialNumber UPCSN,D.Value SN,C.LastUpdate TIME 
                                FROM mesPackage A INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID 
                                inner join mesUnit C on B.UnitID=C.ID
                                inner join mesSerialNumber D on C.ID=D.UnitID
                                where A.Stage=1 and A.SerialNumber= '{SN}'";

        var v_Query = await DapperConn.QueryAsync<CartonBoxConfirmed>(sql, null, null, I_DBTimeout, null);
        return v_Query.ToList();
    }
    public async Task<mesPackage> GetMesPackageBySNAsync(string SN)
    {
        string sql =
                @$"SELECT top 1 *
                    FROM dbo.mesPackage
                    WHERE SerialNumber = '{SN}'";

        var v_Query = await DapperConn.QueryFirstOrDefaultAsync<mesPackage>(sql, null, null, I_DBTimeout, null);
        return v_Query;
    }
    public async Task<mesPackage> GetMesPackageByIDAsync(string ID)
    {
        string sql =
            @$"SELECT top 1 *
                FROM dbo.mesPackage
                WHERE  ID = {ID}";

        var v_Query = await DapperConn.QueryFirstOrDefaultAsync<mesPackage>(sql, null, null, I_DBTimeout, null);
        return v_Query;
    }

	/// <summary>
	/// 通过FG/UPC查找箱号
	/// </summary>
	/// <param name="SN"></param>
	/// <returns></returns>
    public async Task<mesPackage> GetMesPackageByUnitSNAsync(string SN)
    {
        string sql = @$"SELECT top 1 a.* 
					FROM dbo.mesPackage a
					JOIN dbo.mesUnitDetail b ON b.InmostPackageID = a.ID
					WHERE b.KitSerialNumber = '{SN}'";

        var v_Query = await DapperConn.QueryFirstOrDefaultAsync<mesPackage>(sql, null, null, I_DBTimeout, null);
        sql = @$"SELECT top 1 a.* 
					FROM dbo.mesPackage a
					JOIN dbo.mesUnitDetail b ON b.InmostPackageID = a.ID
					JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID
					WHERE c.Value = '{SN}'";

        v_Query ??= await DapperConn.QueryFirstOrDefaultAsync<mesPackage>(sql, null, null, I_DBTimeout, null);


        return v_Query;
    }
    public async Task<(string,string)> Get_CreatePackageSN(string S_FormatName, string xmlProdOrder, string xmlPart,
        string xmlStation,
        string xmlExtraData, mesUnit v_mesUnit,  int type)
    {
        var res = (string.Empty, string.Empty);
        var newSn = await base.GetSNRGetNextAsync(S_FormatName, "0", xmlProdOrder, xmlPart,  xmlStation, xmlExtraData);
        if (string.IsNullOrEmpty(newSn))
            return ("20087", "");

        var affectRow = await InsertMesPackage(newSn, v_mesUnit.PartID.ToInt(), v_mesUnit.ProductionOrderID,
            v_mesUnit.StationID, v_mesUnit.EmployeeID, type);

        if (affectRow <= 0)
            return ("20088", "");

        return ("1",newSn);
    }
	/// <summary>
	/// 插入包装条码
	/// </summary>
	/// <param name="S_SN"></param>
	/// <param name="PartID"></param>
	/// <param name="ProductionOrderID"></param>
	/// <param name="StationID"></param>
	/// <param name="EmployeeID"></param>
	/// <param name="type"></param>
	/// <returns></returns>
    public async Task<int> InsertMesPackage(string S_SN, int PartID, int ProductionOrderID, int StationID, int EmployeeID,int type)
    {
        string strSql = @"INSERT INTO mesPackage(ID,SerialNumber,StationID,EmployeeID,CreationTime,StatusID,LastUpdate,Stage,CurrProductionOrderID,CurrPartID)
				                VALUES (ISNULL((SELECT MAX(ID) FROM mesPackage),0)+1,@BoxSN,@stationId,@employeeId,GETDATE(),0,GETDATE(),@type,@prodId,@partId)";

        DynamicParameters dp = new DynamicParameters();
		dp.Add("");
        dp.Add("@BoxSN", S_SN);
        dp.Add("@stationId", StationID);
        dp.Add("@employeeId", EmployeeID);
        dp.Add("@type", type);
        dp.Add("@prodId", ProductionOrderID);
        dp.Add("@partId", PartID);
        return await DapperConn.ExecuteAsync(strSql, dp, null, I_DBTimeout, null);
    }
    /// <summary>
    /// 插入MesPackageHistory生成语句
    /// </summary>
    /// <param name="packageId"></param>
    /// <param name="packageStatusID"></param>
    /// <param name="loginList"></param>
    /// <returns></returns>
    public string InsertMesPackageHistory(int packageId, int packageStatusID, LoginList loginList) =>
		@$"INSERT INTO dbo.mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)
		VALUES( {packageId}, -- PackageID - int
		{packageStatusID}   , -- PackageStatusID - tinyint
		{loginList.StationID}   , -- StationID - int
		{loginList.EmployeeID}   , -- EmployeeID - int
		GETDATE() -- Time - datetime
    )";
    public async Task<string> MesGetFGSNByUPCSNAsync(string UPCSN)
    {
        string strSql = string.Format(@"SELECT B.Value SerialNumber FROM  mesUnitDetail A 
                INNER JOIN mesSerialNumber B ON A.UnitID = B.UnitID WHERE A.KitSerialNumber = '{0}'", UPCSN);

        var vQuery = await DapperConn.ExecuteScalarAsync(strSql, null, null, I_DBTimeout, null);

        return vQuery?.ToString();
    }
    /// <summary>
    /// 中箱移除
    /// 貌似用UPC条码移除是只能在装箱站，通过箱码移除只能在装栈板
    /// </summary>
    /// <param name="SN"></param>
    /// <param name="xmlProductionOrder"></param>
    /// <param name="xmlStation"></param>
    /// <param name="xmlExtraData"></param>
    /// <param name="type">1:UPC 2:MultipackSN</param>
    /// <returns></returns>
    public async Task<KitBoxCheckOutput> uspPackageRemoveSingleAsync(string SN, string xmlProductionOrder, string xmlStation, string xmlExtraData, string type)
    {
        string mSql = $@"		
            BEGIN
				declare	@StatusID				int,
						@BoxID					int,
						@ParentID				int,
						@PartID					int,
						@routeID				int,
						@LineID					int,
						@StationID				int,
						@idoc					int,
						@employeeId				varchar(64),
						@UnitStateID			varchar(64),
						@CurrStateID			varchar(64),
						@ProductionOrderID		INT,
						@PartFamilyID INT,
						@ProductionOrder        INT,
						@MultipackScanOnlyFGSN  VARCHAR(64),
						@BoxSN VARCHAR(200),
						@strSNFormat      nvarchar(200) = '{SN}',				--SN
						@xmlProdOrder     nvarchar(2000) = '{xmlProductionOrder}', 
						@xmlPart          nvarchar(2000) = null,  
						@xmlStation       nvarchar(2000) = '{xmlStation}', 
						@xmlExtraData     nvarchar(2000) = '{xmlExtraData}',
						@strSNbuf         nvarchar(200) = '{type}',				--类型 1:UPC 2:MultipackSN	
						@strOutput        nvarchar(200) ,
						@unitId int
        
				SET @strOutput='1'
				BEGIN TRAN
				BEGIN TRY
					--读取Station参数
					exec sp_xml_preparedocument @idoc output, @xmlStation
					SELECT @StationID=StationId
					FROM   OPENXML (@idoc, '/Station',2)
							WITH (StationId		  int			'@StationId')   
					exec sp_xml_removedocument @idoc

					--读取@employeeId参数
					exec sp_xml_preparedocument @idoc output, @xmlExtraData
					SELECT @employeeId=EmployeeId
					FROM   OPENXML (@idoc, '/ExtraData',2)
							WITH (EmployeeId	  int			'@EmployeeId')   
					exec sp_xml_removedocument @idoc
        
					exec sp_xml_preparedocument @idoc output, @xmlProdOrder
   
					SELECT @ProductionOrder=ProductionOrder
					FROM   OPENXML (@idoc, '/ProdOrder',2)
							WITH (ProductionOrder  int		'@ProductionOrder'
								)   
   
					exec sp_xml_removedocument @idoc


					CREATE TABLE #TempRoute
					(	
						ID				int,
						UnitStateID		int
					)

        
					IF @strSNbuf='1'
					BEGIN
						SELECT @MultipackScanOnlyFGSN = a.Content
						FROM dbo.mesProductionOrderDetail a
						JOIN dbo.luProductionOrderDetailDef b ON a.ProductionOrderDetailDefID = b.ID
						WHERE ProductionOrderID = @ProductionOrder AND b.Description = 'MultipackScanOnlyFGSN'
						IF @MultipackScanOnlyFGSN = '1'
						BEGIN
							SELECT @BoxID=A.InmostPackageID,@ParentID=B.ParentID FROM mesUnitDetail A
							INNER JOIN mesPackage B ON A.InmostPackageID=B.ID 
							JOIN dbo.mesSerialNumber c ON c.UnitID = a.UnitID
							WHERE c.Value = @strSNFormat
						end
						else
						begin
							SELECT @BoxID=A.InmostPackageID,@ParentID=B.ParentID FROM mesUnitDetail A
							INNER JOIN mesPackage B ON A.InmostPackageID=B.ID 
							WHERE A.KitSerialNumber = @strSNFormat
						end
						IF ISNULL(@BoxID,'')=''
						BEGIN
							SET @strOutput='20119'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							ROLLBACK TRAN
							RETURN
						END

						IF ISNULL(@ParentID,'')<>''
						BEGIN
							SET @strOutput='20003'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							ROLLBACK TRAN
							RETURN
						END
						IF @MultipackScanOnlyFGSN = '1'
						BEGIN
							SELECT TOP 1 @CurrStateID=UnitStateID,@PartID=B.PartID,@LineID=B.LineID,@ProductionOrderID=b.ProductionOrderID,@unitId = b.ID
							FROM mesUnitDetail A INNER JOIN mesUnit B
								ON A.UnitID=B.ID 
							JOIN dbo.mesSerialNumber c ON c.UnitID = b.ID
							WHERE c.Value = @strSNFormat
						end
						else
						begin
							SELECT TOP 1 @CurrStateID=UnitStateID,@PartID=B.PartID,@LineID=B.LineID,@ProductionOrderID=b.ProductionOrderID,@unitId = b.ID
							FROM mesUnitDetail A INNER JOIN mesUnit B
								ON A.UnitID=B.ID WHERE A.KitSerialNumber=@strSNFormat
						end

						--SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
						select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID

						select top 1 @routeID = RouteID
						from mesRouteMap
						where
							(LineID = @LineID or LineID is NULL OR LineID=0) AND
							(PartID = @PartID or PartID is NULL OR PartID=0) AND
							(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
							(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
						order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc

						IF ISNULL(@routeID,'')=''
						BEGIN
							SET @strOutput='20195'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							ROLLBACK TRAN
							RETURN
						END
						--根据route类型获取上一站UnitstateID
						IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
						BEGIN
							IF(SELECT COUNT(1) FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
								WHERE RouteID=@routeID)>1
							BEGIN
								if not exists( select 1 from mesHistory where UnitID = @unitId and UnitStateID in (SELECT a.CurrStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
								WHERE RouteID=@routeID))
								begin
									SET @strOutput='20203'
									SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
									ROLLBACK TRAN
									RETURN
								end

								--通过历史记录表查询最大ID的工序					
								SELECT TOP 1 @UnitStateID = c.UnitStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
								JOIN dbo.mesHistory c ON c.UnitID = @unitId AND a.CurrStateID = c.UnitStateID
								WHERE a.RouteID=@routeID
								ORDER BY c.ID DESC
							END

							SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
							WHERE RouteID=@routeID
						END
						ELSE
						BEGIN
							INSERT INTO #TempRoute(ID,UnitStateID)
							SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
							FROM mesRouteDetail where RouteID=@routeID

							SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
								(SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
						END

						IF ISNULL(@UnitStateID,'')=''
						BEGIN
							SET @strOutput='20131'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							ROLLBACK TRAN
							RETURN 
						END
						IF @MultipackScanOnlyFGSN = '1'
						BEGIN
							UPDATE A SET UnitStateID=@UnitStateID FROM mesUnit A INNER JOIN mesUnitDetail B
							ON A.ID=B.UnitID 
							JOIN dbo.mesSerialNumber c ON c.UnitID = b.ID
							WHERE c.Value = @strSNFormat

							UPDATE b SET InmostPackageID=NULL 
							FROM dbo.mesUnitDetail b
							JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID
							WHERE c.Value = @strSNFormat

							UPDATE b SET reserved_18 = ISNULL(reserved_18, '') +','+ CAST(@BoxID AS VARCHAR(30)) + '-'+ CAST(GETDATE() AS VARCHAR(30))
							FROM dbo.mesUnitDetail b
							JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID
							WHERE c.Value = @strSNFormat

							UPDATE mesPackage SET CurrentCount=CASE WHEN ISNULL(CurrentCount,0)=0 THEN NULL ELSE CurrentCount-1 END,StatusID=0 WHERE ID=@BoxID

							INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
							SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesUnitDetail A 
								INNER JOIN mesUnit B ON A.UnitID=B.ID
								JOIN dbo.mesSerialNumber c ON c.UnitID = b.ID
							WHERE c.Value = @strSNFormat
						end
						else
						begin
							UPDATE A SET UnitStateID=@UnitStateID FROM mesUnit A INNER JOIN mesUnitDetail B
								ON A.ID=B.UnitID WHERE B.KitSerialNumber=@strSNFormat

							UPDATE mesUnitDetail SET InmostPackageID=NULL WHERE KitSerialNumber=@strSNFormat
							---20220519 
							UPDATE dbo.mesUnitDetail SET reserved_18 = ISNULL(reserved_18, '') +','+ CAST(@BoxID AS VARCHAR(30)) + '-'+ CAST(GETDATE() AS VARCHAR(30)) WHERE KitSerialNumber = @strSNFormat
							---

							UPDATE mesPackage SET CurrentCount=CASE WHEN ISNULL(CurrentCount,0)=0 THEN NULL ELSE CurrentCount-1 END,StatusID=0 WHERE ID=@BoxID

							INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
							SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesUnitDetail A 
								INNER JOIN mesUnit B ON A.UnitID=B.ID
							WHERE A.KitSerialNumber=@strSNFormat

            

							--判断UPC和FG是不是同一个条码
							IF NOT EXISTS(SELECT * FROM mesUnitDetail A INNER JOIN mesSerialNumber B ON  A.UnitID=B.UnitID 
										WHERE A.KitSerialNumber=@strSNFormat AND B.Value=@strSNFormat)
							BEGIN
								UPDATE A SET A.UnitStateID=@UnitStateID FROM mesUnit A INNER JOIN mesSerialNumber B ON A.ID=B.UnitID
									WHERE B.Value=@strSNFormat

								INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
								SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesSerialNumber A 
									INNER JOIN mesUnit B ON A.UnitID=B.ID
								WHERE A.Value=@strSNFormat
							END 
						end
						IF EXISTS(SELECT 1 FROM dbo.mesPackage WHERE ID = @BoxID AND Stage = 1 AND ISNULL(@ParentID,0) = 0)
						BEGIN
							SELECT @BoxSN = SerialNumber FROM dbo.mesPackage WHERE ID = @BoxID AND Stage = 1 AND ISNULL(@ParentID,0) = 0
						END
					END
					ELSE IF @strSNbuf='2'
					BEGIN
						SELECT top 1 @BoxID=A.ID,@StatusID=A.StatusID,@ParentID=ParentID,
							@PartID=A.CurrPartID,@LineID=B.LineID,@ProductionOrderID=A.CurrProductionOrderID
						FROM mesPackage A INNER JOIN mesStation B on A.StationID=B.ID
						WHERE SerialNumber = @strSNFormat AND Stage=1

						IF ISNULL(@BoxID,'')='' 
						BEGIN
							SET @strOutput='20119'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							ROLLBACK TRAN
							RETURN
						END

						IF @StatusID=0 OR ISNULL(@ParentID,'')=''
						BEGIN
							SET @strOutput='20003'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							ROLLBACK TRAN
							RETURN
						END
            
						--SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
						select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID

						select top 1 @routeID = RouteID
						from mesRouteMap
						where
							(LineID = @LineID or LineID is NULL OR LineID=0) AND
							(PartID = @PartID or PartID is NULL OR PartID=0) AND
							(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
							(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
						order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc

						IF ISNULL(@routeID,'')=''
						BEGIN
							SET @strOutput='20195'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							ROLLBACK TRAN
							RETURN
						END

						SELECT TOP 1 @CurrStateID=UnitStateID FROM mesUnitDetail A INNER JOIN mesUnit B
						ON A.UnitID=B.ID WHERE InmostPackageID=@BoxID
						---
						IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
						BEGIN
							IF(SELECT COUNT(1) FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
								WHERE RouteID=@routeID)>1
							BEGIN
								SET @strOutput='20203'
								SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
								RETURN
							END

							SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
							WHERE RouteID=@routeID
						END
						ELSE
						BEGIN
							INSERT INTO #TempRoute(ID,UnitStateID)
							SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
							FROM mesRouteDetail where RouteID=@routeID

							SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
								(SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
						END
						--

						IF ISNULL(@UnitStateID,'')=''
						BEGIN
							SET @strOutput='20131'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							RETURN 
						END

						UPDATE mesPackage SET CurrentCount=CASE WHEN ISNULL(CurrentCount,0)=0 THEN NULL ELSE CurrentCount-1 END,StatusID=0 WHERE ID=@ParentID
						UPDATE mesPackage SET ParentID = NULL WHERE ID=@BoxID

						UPDATE A SET A.UnitStateID=@UnitStateID,A.LastUpdate=GETDATE() FROM mesUnit A
							INNER JOIN mesUnitDetail B ON A.ID=B.UnitID WHERE ISNULL(B.InmostPackageID,'')= @BoxID
						UPDATE A SET A.UnitStateID=@UnitStateID,A.LastUpdate=GETDATE() FROM mesUnit A
							INNER JOIN mesSerialNumber C ON A.ID=C.UnitID
							INNER JOIN mesUnitDetail B ON C.Value=B.KitSerialNumber WHERE ISNULL(B.InmostPackageID,'')= @BoxID
						---20220519 
						UPDATE B SET b.reserved_18 = ISNULL(reserved_18, '') +','+ @strSNFormat + '-'+ CAST(GETDATE() AS VARCHAR(30))  from mesUnitDetail B WHERE ISNULL(B.InmostPackageID,'')= @BoxID
						---
						INSERT INTO mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,[TIME])
						VALUES (@BoxID,4,@StationID,@employeeId,GETDATE())
					END
					ELSE
					BEGIN
						SET @strOutput='20085'
						SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
						ROLLBACK TRAN
						RETURN
					END
					COMMIT TRAN
					SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
					DROP TABLE #TempRoute
				END TRY
				BEGIN CATCH
					ROLLBACK TRAN
					SET @strOutput = ERROR_MESSAGE()
					SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
					RETURN
				END CATCH
			END
            ";
        

         return await DapperConn.QueryFirstAsync<KitBoxCheckOutput>(mSql,null,null,I_DBTimeout,null);
    }
    /// <summary>
    /// 中箱移除
    /// 貌似用UPC条码移除是只能在装箱站，通过箱码移除只能在装栈板
	/// 备份
    /// </summary>
    /// <param name="SN"></param>
    /// <param name="xmlProductionOrder"></param>
    /// <param name="xmlStation"></param>
    /// <param name="xmlExtraData"></param>
    /// <param name="type">1:UPC 2:MultipackSN</param>
    /// <returns></returns>
    public async Task<KitBoxCheckOutput> uspPackageRemoveSingleAsync_P(string SN, string xmlProductionOrder, string xmlStation, string xmlExtraData, string type)
    {
        string mSql = $@"		
            BEGIN
	            declare	@StatusID				int,
			            @BoxID					int,
			            @ParentID				int,
			            @PartID					int,
			            @routeID				int,
			            @LineID					int,
			            @StationID				int,
			            @idoc					int,
			            @employeeId				varchar(64),
			            @UnitStateID			varchar(64),
			            @CurrStateID			varchar(64),
			            @ProductionOrderID		INT,
			            @ProductionOrder        INT,
			            @MultipackScanOnlyFGSN  VARCHAR(64),
                        @BoxSN VARCHAR(200),
			            @strSNFormat      nvarchar(200) = '{SN}',				--SN
			            @xmlProdOrder     nvarchar(2000) = '{xmlProductionOrder}', 
			            @xmlPart          nvarchar(2000) = null,  
			            @xmlStation       nvarchar(2000) = '{xmlStation}', 
			            @xmlExtraData     nvarchar(2000) = '{xmlExtraData}',
			            @strSNbuf         nvarchar(200) = '{type}',				--类型 1:UPC 2:MultipackSN	
			            @strOutput        nvarchar(200) ,
                        @unitId int
		            
	            SET @strOutput='1'
	            BEGIN TRAN
	            BEGIN TRY
		            --读取Station参数
		            exec sp_xml_preparedocument @idoc output, @xmlStation
		            SELECT @StationID=StationId
		            FROM   OPENXML (@idoc, '/Station',2)
				            WITH (StationId		  int			'@StationId')   
		            exec sp_xml_removedocument @idoc

		            --读取@employeeId参数
		            exec sp_xml_preparedocument @idoc output, @xmlExtraData
		            SELECT @employeeId=EmployeeId
		            FROM   OPENXML (@idoc, '/ExtraData',2)
				            WITH (EmployeeId	  int			'@EmployeeId')   
		            exec sp_xml_removedocument @idoc
		            
		            exec sp_xml_preparedocument @idoc output, @xmlProdOrder
               
		            SELECT @ProductionOrder=ProductionOrder
		            FROM   OPENXML (@idoc, '/ProdOrder',2)
				            WITH (ProductionOrder  int		'@ProductionOrder'
					            )   
               
		            exec sp_xml_removedocument @idoc


		            CREATE TABLE #TempRoute
		            (	
			            ID				int,
			            UnitStateID		int
		            )

		            
		            IF @strSNbuf='1'
		            BEGIN
			            SELECT @MultipackScanOnlyFGSN = a.Content
			            FROM dbo.mesProductionOrderDetail a
			            JOIN dbo.luProductionOrderDetailDef b ON a.ProductionOrderDetailDefID = b.ID
			            WHERE ProductionOrderID = @ProductionOrder AND b.Description = 'MultipackScanOnlyFGSN'
						IF @MultipackScanOnlyFGSN = '1'
			            BEGIN
							SELECT @BoxID=A.InmostPackageID,@ParentID=B.ParentID FROM mesUnitDetail A
				            INNER JOIN mesPackage B ON A.InmostPackageID=B.ID 
				            JOIN dbo.mesSerialNumber c ON c.UnitID = a.UnitID
				            WHERE c.Value = @strSNFormat
						end
						else
						begin
						    SELECT @BoxID=A.InmostPackageID,@ParentID=B.ParentID FROM mesUnitDetail A
				            INNER JOIN mesPackage B ON A.InmostPackageID=B.ID 
							WHERE A.KitSerialNumber = @strSNFormat
						end
						IF ISNULL(@BoxID,'')=''
				        BEGIN
					        SET @strOutput='20119'
					        SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
					        ROLLBACK TRAN
					        RETURN
				        END

				        IF ISNULL(@ParentID,'')<>''
				        BEGIN
					        SET @strOutput='20003'
					        SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
					        ROLLBACK TRAN
					        RETURN
				        END
						IF @MultipackScanOnlyFGSN = '1'
			            BEGIN
				            SELECT TOP 1 @CurrStateID=UnitStateID,@PartID=B.PartID,@LineID=B.LineID,@ProductionOrderID=b.ProductionOrderID,@unitId = b.ID
				            FROM mesUnitDetail A INNER JOIN mesUnit B
					            ON A.UnitID=B.ID 
				            JOIN dbo.mesSerialNumber c ON c.UnitID = b.ID
				            WHERE c.Value = @strSNFormat
						end
						else
						begin
				            SELECT TOP 1 @CurrStateID=UnitStateID,@PartID=B.PartID,@LineID=B.LineID,@ProductionOrderID=b.ProductionOrderID,@unitId = b.ID
				            FROM mesUnitDetail A INNER JOIN mesUnit B
					            ON A.UnitID=B.ID WHERE A.KitSerialNumber=@strSNFormat
						end
						SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
				        IF ISNULL(@routeID,'')=''
				        BEGIN
					        SET @strOutput='20195'
					        SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
					        ROLLBACK TRAN
					        RETURN
				        END
				        --根据route类型获取上一站UnitstateID
				        IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
				        BEGIN
					        IF(SELECT COUNT(1) FROM mesUnitInputState A 
						        INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
						        WHERE RouteID=@routeID)>1
					        BEGIN
								if not exists( select 1 from mesHistory where UnitID = @unitId and UnitStateID in (SELECT a.CurrStateID FROM mesUnitInputState A 
						        INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
						        WHERE RouteID=@routeID))
								begin
									SET @strOutput='20203'
									SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
									ROLLBACK TRAN
									RETURN
								end

								--通过历史记录表查询最大ID的工序					
								SELECT TOP 1 @UnitStateID = c.UnitStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
								JOIN dbo.mesHistory c ON c.UnitID = @unitId AND a.CurrStateID = c.UnitStateID
								WHERE a.RouteID=@routeID
								ORDER BY c.ID DESC
					        END

					        SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
						        INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
					        WHERE RouteID=@routeID
				        END
				        ELSE
				        BEGIN
					        INSERT INTO #TempRoute(ID,UnitStateID)
					        SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
					        FROM mesRouteDetail where RouteID=@routeID

					        SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
						        (SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
				        END

				        IF ISNULL(@UnitStateID,'')=''
				        BEGIN
					        SET @strOutput='20131'
					        SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
					        ROLLBACK TRAN
					        RETURN 
				        END
						IF @MultipackScanOnlyFGSN = '1'
			            BEGIN
							UPDATE A SET UnitStateID=@UnitStateID FROM mesUnit A INNER JOIN mesUnitDetail B
				            ON A.ID=B.UnitID 
				            JOIN dbo.mesSerialNumber c ON c.UnitID = b.ID
				            WHERE c.Value = @strSNFormat

				            UPDATE b SET InmostPackageID=NULL 
				            FROM dbo.mesUnitDetail b
				            JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID
				            WHERE c.Value = @strSNFormat

				            UPDATE b SET reserved_18 = ISNULL(reserved_18, '') +','+ CAST(@BoxID AS VARCHAR(30)) + '-'+ CAST(GETDATE() AS VARCHAR(30))
				            FROM dbo.mesUnitDetail b
				            JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID
				            WHERE c.Value = @strSNFormat

				            UPDATE mesPackage SET CurrentCount=CASE WHEN ISNULL(CurrentCount,0)=0 THEN NULL ELSE CurrentCount-1 END,StatusID=0 WHERE ID=@BoxID

				            INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
				            SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesUnitDetail A 
					            INNER JOIN mesUnit B ON A.UnitID=B.ID
					            JOIN dbo.mesSerialNumber c ON c.UnitID = b.ID
				            WHERE c.Value = @strSNFormat
						end
						else
						begin
							UPDATE A SET UnitStateID=@UnitStateID FROM mesUnit A INNER JOIN mesUnitDetail B
					            ON A.ID=B.UnitID WHERE B.KitSerialNumber=@strSNFormat

				            UPDATE mesUnitDetail SET InmostPackageID=NULL WHERE KitSerialNumber=@strSNFormat
				            ---20220519 
				            UPDATE dbo.mesUnitDetail SET reserved_18 = ISNULL(reserved_18, '') +','+ CAST(@BoxID AS VARCHAR(30)) + '-'+ CAST(GETDATE() AS VARCHAR(30)) WHERE KitSerialNumber = @strSNFormat
				            ---

				            UPDATE mesPackage SET CurrentCount=CASE WHEN ISNULL(CurrentCount,0)=0 THEN NULL ELSE CurrentCount-1 END,StatusID=0 WHERE ID=@BoxID

				            INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
				            SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesUnitDetail A 
					            INNER JOIN mesUnit B ON A.UnitID=B.ID
				            WHERE A.KitSerialNumber=@strSNFormat

			            

				            --判断UPC和FG是不是同一个条码
				            IF NOT EXISTS(SELECT * FROM mesUnitDetail A INNER JOIN mesSerialNumber B ON  A.UnitID=B.UnitID 
							            WHERE A.KitSerialNumber=@strSNFormat AND B.Value=@strSNFormat)
				            BEGIN
					            UPDATE A SET A.UnitStateID=@UnitStateID FROM mesUnit A INNER JOIN mesSerialNumber B ON A.ID=B.UnitID
						            WHERE B.Value=@strSNFormat

					            INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
					            SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesSerialNumber A 
						            INNER JOIN mesUnit B ON A.UnitID=B.ID
					            WHERE A.Value=@strSNFormat
				            END 
						end
			            IF EXISTS(SELECT 1 FROM dbo.mesPackage WHERE ID = @BoxID AND Stage = 1 AND ISNULL(@ParentID,0) = 0)
			            BEGIN
			                SELECT @BoxSN = SerialNumber FROM dbo.mesPackage WHERE ID = @BoxID AND Stage = 1 AND ISNULL(@ParentID,0) = 0
			            END
		            END
		            ELSE IF @strSNbuf='2'
		            BEGIN
			            SELECT top 1 @BoxID=A.ID,@StatusID=A.StatusID,@ParentID=ParentID,
				            @PartID=A.CurrPartID,@LineID=B.LineID,@ProductionOrderID=A.CurrProductionOrderID
			            FROM mesPackage A INNER JOIN mesStation B on A.StationID=B.ID
			            WHERE SerialNumber = @strSNFormat AND Stage=1

			            IF ISNULL(@BoxID,'')='' 
			            BEGIN
				            SET @strOutput='20119'
				            SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
				            ROLLBACK TRAN
					        RETURN
			            END

			            IF @StatusID=0 OR ISNULL(@ParentID,'')=''
			            BEGIN
				            SET @strOutput='20003'
				            SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
				            ROLLBACK TRAN
					        RETURN
			            END
			            
			            SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
			            IF ISNULL(@routeID,'')=''
			            BEGIN
				            SET @strOutput='20195'
				            SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
				            ROLLBACK TRAN
					        RETURN
			            END

			            SELECT TOP 1 @CurrStateID=UnitStateID FROM mesUnitDetail A INNER JOIN mesUnit B
			            ON A.UnitID=B.ID WHERE InmostPackageID=@BoxID
						---
						IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
						BEGIN
							IF(SELECT COUNT(1) FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
								WHERE RouteID=@routeID)>1
							BEGIN
								SET @strOutput='20203'
								SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
								RETURN
							END

							SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID
							WHERE RouteID=@routeID
						END
						ELSE
						BEGIN
							INSERT INTO #TempRoute(ID,UnitStateID)
							SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
							FROM mesRouteDetail where RouteID=@routeID

							SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
								(SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
						END
						--

						IF ISNULL(@UnitStateID,'')=''
						BEGIN
							SET @strOutput='20131'
							SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
							RETURN 
						END

			            UPDATE mesPackage SET CurrentCount=CASE WHEN ISNULL(CurrentCount,0)=0 THEN NULL ELSE CurrentCount-1 END,StatusID=0 WHERE ID=@ParentID
			            UPDATE mesPackage SET ParentID = NULL WHERE ID=@BoxID

			            UPDATE A SET A.UnitStateID=@UnitStateID,A.LastUpdate=GETDATE() FROM mesUnit A
				            INNER JOIN mesUnitDetail B ON A.ID=B.UnitID WHERE ISNULL(B.InmostPackageID,'')= @BoxID
			            UPDATE A SET A.UnitStateID=@UnitStateID,A.LastUpdate=GETDATE() FROM mesUnit A
				            INNER JOIN mesSerialNumber C ON A.ID=C.UnitID
				            INNER JOIN mesUnitDetail B ON C.Value=B.KitSerialNumber WHERE ISNULL(B.InmostPackageID,'')= @BoxID
			            ---20220519 
			            UPDATE B SET b.reserved_18 = ISNULL(reserved_18, '') +','+ @strSNFormat + '-'+ CAST(GETDATE() AS VARCHAR(30))  from mesUnitDetail B WHERE ISNULL(B.InmostPackageID,'')= @BoxID
			            ---
			            INSERT INTO mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,[TIME])
			            VALUES (@BoxID,4,@StationID,@employeeId,GETDATE())
		            END
		            ELSE
		            BEGIN
			            SET @strOutput='20085'
			            SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
			            ROLLBACK TRAN
					    RETURN
		            END
		            COMMIT TRAN
		            SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
		            DROP TABLE #TempRoute
	            END TRY
	            BEGIN CATCH
		            ROLLBACK TRAN
		            SET @strOutput = ERROR_MESSAGE()
		            SELECT @strOutput AS strOutput, @BoxSN AS BoxSN
		            RETURN
	            END CATCH
            END
            ";


        return await DapperConn.QueryFirstAsync<KitBoxCheckOutput>(mSql, null, null, I_DBTimeout, null);
    }
    public async Task<string> CheckExistsLinkedAsync(string SN, string boxSn, string POID)
    {
		string strSql = $@"
						DECLARE @isFg VARCHAR(200),@result VARCHAR(200) = '0',@PoID INT = '{POID}',@SN VARCHAR(200) = '{SN}',@boxSn VARCHAR(200)='{boxSn}'
						SELECT @isFg = LTRIM(RTRIM(a.Content))
						FROM dbo.mesProductionOrderDetail a
						JOIN dbo.luProductionOrderDetailDef b ON b.ID= a.ProductionOrderDetailDefID
						WHERE b.Description = 'MultipackScanOnlyFGSN' AND a.ID = @PoID

						IF @isFg = '1'
						BEGIN
							IF EXISTS(	
							SELECT 1 
							FROM dbo.mesPackage a
							JOIN dbo.mesUnitDetail b ON b.InmostPackageID = a.ID
							JOIN dbo.mesSerialNumber  c ON c.UnitID = b.UnitID
							WHERE a.SerialNumber = @boxSn AND c.Value = @SN)
							BEGIN
								SELECT @result = '1'
							END
						END
						ELSE
						BEGIN
							IF EXISTS(
								SELECT 1 
								FROM dbo.mesPackage a
								JOIN dbo.mesUnitDetail b ON b.InmostPackageID = a.ID
								WHERE a.SerialNumber = @boxSn AND b.KitSerialNumber= @SN)
							BEGIN
								SELECT @result = '1'
							END
						END
						SELECT @result";

        var vQuery = await DapperConn.ExecuteScalarAsync(strSql, null, null, I_DBTimeout, null);

        return vQuery?.ToString();
    }
    public async Task<string> uspPalletCheckAsync(string S_FormatName, string xmlProdOrder, string xmlPart, string xmlStation, string xmlExtraData, string strSNbuf)
    {
        string strSql = $@"
							
BEGIN
	declare	@PalletId	int,
			@PalletPartID	int,
			@idoc		int,
			@txtPart    varchar(8000),
			@partId		int,
			@prodId		int,
			@isMixedPN  int,
			@isMixedPO	int,
			@stationId  int,
			@employeeId INT,
			@strSNFormat      nvarchar(200) = '{S_FormatName}',		
			@xmlProdOrder     nvarchar(2000) = '{xmlProdOrder}', 
			@xmlPart          nvarchar(2000) = '{xmlPart}',  
			@xmlStation       nvarchar(2000) = '{xmlStation}', 
			@xmlExtraData     nvarchar(2000) = '{xmlExtraData}',
			@strSNbuf         nvarchar(200) = '{strSNbuf}',			--传入参数类型PALLET/BOX
			@strOutput        nvarchar(200) 
		BEGIN TRY


		--读取Part参数值
		set @txtPart=cast(@xmlPart as NVARCHAR)
		exec sp_xml_preparedocument @idoc output, @xmlPart
		SELECT @partId=PartId
		FROM   OPENXML (@idoc,			'/Part',2)
				WITH (PartId  int		'@PartID'
					)   
		exec sp_xml_removedocument @idoc
		IF isnull(@partId,'') = ''
		begin
			SELECT '20077'
			RETURN
		end

		--读取ProdOrder参数值
		exec sp_xml_preparedocument @idoc output, @xmlProdOrder
		SELECT @prodId=ProdId
		FROM   OPENXML (@idoc,			'/ProdOrder',2)
				WITH (ProdId  int		'@ProdOrderID')   
		exec sp_xml_removedocument @idoc

		IF isnull(@prodId,'') = ''
		begin
			SELECT '20077'
			RETURN
		end

		IF @strSNbuf='PALLET'
		BEGIN
			IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat)
			BEGIN

				--读取Station参数
				exec sp_xml_preparedocument @idoc output, @xmlStation
				SELECT @stationId=StationId
				FROM   OPENXML (@idoc, '/Station',2)
						WITH (StationId		  int			'@StationId')   
				exec sp_xml_removedocument @idoc

				--读取@employeeId参数
				exec sp_xml_preparedocument @idoc output, @xmlExtraData
				SELECT @employeeId=EmployeeId
				FROM   OPENXML (@idoc, '/ExtraData',2)
						WITH (EmployeeId	  int			'@EmployeeId')   
				exec sp_xml_removedocument @idoc

				INSERT INTO mesPackage(ID,SerialNumber,StationID,EmployeeID,CreationTime,StatusID,LastUpdate,Stage,CurrProductionOrderID,CurrPartID)
				VALUES
				((SELECT isnull(MAX(ID),0) FROM mesPackage)+1,@strSNFormat,@stationId,@employeeId,GETDATE(),0,GETDATE(),2,@prodId,@partId)
			END
			ELSE
			BEGIN
				IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat AND Stage=2)
				BEGIN
					SELECT '20035'
					RETURN
				END
				--读取ProdOrder参数 20201008 Add 
				exec sp_xml_preparedocument @idoc output, @xmlProdOrder
				SELECT @prodId=prodId
				FROM   OPENXML (@idoc, '/ProdOrder',2)
						WITH (prodId		  int			'@ProdOrderID')   
				exec sp_xml_removedocument @idoc
				--判断箱号是否和选择工单一致
				IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat AND CurrProductionOrderID = @prodId)
				BEGIN
					SELECT '20133'
					RETURN
				END
			END
		END
		ELSE IF @strSNbuf='BOX'
		BEGIN
			IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat and Stage=1 and StatusID=1)
			BEGIN
				SELECT '20119'
				RETURN
			END

			IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat and ISNULL(ParentID,'')<>'')
			BEGIN
				SELECT '20183'
				RETURN
			END
			
			
			--------------------------------20220903
			IF	EXISTS(	SELECT b.ID
						FROM dbo.mesPackage a
						JOIN dbo.mesUnitDetail b ON b.InmostPackageID = a.ID
						JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID AND c.SerialNumberTypeID = 6
						WHERE a.SerialNumber = @strSNFormat)
				OR 
				EXISTS(SELECT b.ID 
						FROM dbo.mesPackage a
						JOIN dbo.mesUnitDetail b ON b.InmostPackageID = a.ID
						JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID AND c.SerialNumberTypeID = 5
						WHERE a.SerialNumber = @strSNFormat AND ISNULL(b.InmostPackageID,'') = '')
			BEGIN
				SELECT '70051'
				RETURN	    
			END

			DECLARE @MultipackFGCount INT, @MultipackPackCount INT
			SELECT @MultipackFGCount= isnull(COUNT(1),0)
			FROM dbo.mesPackage a
			JOIN dbo.mesUnitDetail b ON b.InmostPackageID = a.ID
			JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID AND c.SerialNumberTypeID = 5    --FG
			--JOIN dbo.mesHistory d ON d.UnitID = c.UnitID AND d.UnitStateID = 300
			--JOIN dbo.mesSerialNumber cupc ON cupc.Value = b.KitSerialNumber AND cupc.SerialNumberTypeID = 6  --UPC
			WHERE a.SerialNumber = @strSNFormat


			SELECT @MultipackPackCount= isnull(a.CurrentCount,0)
			FROM dbo.mesPackage a
			--JOIN dbo.mesUnitDetail b ON b.InmostPackageID = a.ID
			--JOIN dbo.mesSerialNumber c ON c.UnitID = b.UnitID AND c.SerialNumberTypeID = 5    --FG
			--left JOIN dbo.mesSerialNumber cupc ON cupc.Value = b.KitSerialNumber AND cupc.SerialNumberTypeID = 6  --UPC
			WHERE a.SerialNumber = @strSNFormat 

			if (@MultipackFGCount <> CAST(@MultipackPackCount AS INT))
			begin
			SELECT '70051'
				RETURN	 
			end

			DECLARE @MultipackCountSPEC VARCHAR(20)
			SELECT @MultipackCountSPEC = c.Content
			FROM dbo.mesPackage a
			JOIN dbo.mesProductionOrderDetail c ON c.ProductionOrderID = a.CurrProductionOrderID
			JOIN dbo.luProductionOrderDetailDef d ON d.ID = c.ProductionOrderDetailDefID AND d.Description = 'BoxQty'
			WHERE a.SerialNumber = @strSNFormat

			DECLARE @MultipackIsLastBoxSPEC VARCHAR(20)
			select @MultipackIsLastBoxSPEC=isnull(std.Content,0) from mesStation s 
			join mesStationType st on s.StationTypeID=st.ID 
			join mesStationTypeDetail std on std.StationTypeID=st.ID and std.StationTypeDetailDefID=(select id from luStationTypeDetailDef where Description='IsLastBox')
			where s.ID=@stationId
			if @MultipackIsLastBoxSPEC='0'
			begin
			   if (@MultipackFGCount <> CAST(@MultipackCountSPEC AS INT) or @MultipackPackCount <> CAST(@MultipackCountSPEC AS INT))
			   begin
			   SELECT '70051'
				RETURN	 
				end
			end

			
			----------------------------------
			--读取ProdOrder参数值
			exec sp_xml_preparedocument @idoc output, @xmlProdOrder
			SELECT @prodId=ProdId
			FROM   OPENXML (@idoc,			'/ProdOrder',2)
					WITH (ProdId  int		'@ProdOrderID')   
			exec sp_xml_removedocument @idoc
			IF isnull(@prodId,'') = ''
			begin
				SELECT '20077'
				RETURN
			end

			--获取配置是否可以混料号/工单
			SELECT @isMixedPO = A.Content FROM mesProductionOrderDetail A INNER JOIN luProductionOrderDetailDef B
				ON A.ProductionOrderDetailDefID=B.ID AND B.Description='IsMixedPO' AND A.ProductionOrderID=@prodId

			SELECT @isMixedPN = A.Content FROM mesProductionOrderDetail A INNER JOIN luProductionOrderDetailDef B
				ON A.ProductionOrderDetailDefID=B.ID AND B.Description='IsMixedPN' AND A.ProductionOrderID=@prodId
			
			--不能混料号进行校验
			IF isnull(@isMixedPN,0)<>1
			BEGIN
				IF NOT EXISTS( SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat AND CurrPartID=@partId)
				BEGIN
					SELECT '20043'
					RETURN
				END
			END
			--不能混工单进行校验
			IF isnull(@isMixedPO,0)<>1
			BEGIN
				IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat AND CurrProductionOrderID=@prodId)
				BEGIN
					SELECT '20050'
					RETURN
				END
			END
		END
		ELSE
		BEGIN
			SELECT '20085'
			RETURN
		END

	SELECT 1
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()
	END CATCH
END


";

        var vQuery = await DapperConn.ExecuteScalarAsync(strSql, null, null, I_DBTimeout, null);

        return vQuery?.ToString();
    }    

	/// <summary>
	/// 获取栈板数据
	/// </summary>
	/// <param name="SN"></param>
	/// <returns></returns>
    public async Task<List<PalletConfirmed>> GetPalletDataAsync(string SN)
    {
        string strSql = $@"
						SELECT ROW_NUMBER() OVER(ORDER BY A.ID) SEQNO, B.SerialNumber KITSN,B.LastUpdate TIME 
	                            FROM mesPackage A INNER JOIN mesPackage B ON A.ID=B.ParentID WHERE A.Stage=2 AND A.SerialNumber= '{SN}'";

        var vQuery = await DapperConn.QueryAsync<PalletConfirmed>(strSql, null, null, I_DBTimeout, null);
        return vQuery?.ToList();
    }

	/// <summary>
	/// 获取料号及料号组属性
	/// </summary>
	/// <param name="PartId"></param>
	/// <param name="PartDetailDefName"></param>
	/// <param name="PartFamilyDetailDefName"></param>
	/// <param name="result"></param>
	/// <returns></returns>
    public async Task<(string, ScalageConfig)> GetMesScalagePartAndPartFamilyDetail(int PartId, string PartDetailDefName, string PartFamilyDetailDefName = "")
    {
        try
        {
            if (string.IsNullOrEmpty(PartDetailDefName) && string.IsNullOrEmpty(PartFamilyDetailDefName))
                return ("70022",null);


            PartFamilyDetailDefName = string.IsNullOrEmpty(PartFamilyDetailDefName) ? PartDetailDefName : PartFamilyDetailDefName;
            PartDetailDefName = string.IsNullOrEmpty(PartDetailDefName) ? PartFamilyDetailDefName : PartDetailDefName;


			var dynParams = new DynamicParameters();
			dynParams.Add("PartId", PartId);
			dynParams.Add("PartDetailDefName", PartDetailDefName);
			dynParams.Add("PartFamilyDetailDefName", PartFamilyDetailDefName);

            var scalageConfig = await DapperConn.QueryAsync<ItemValues>(@"SELECT 'PartFamily'  Item,  a.Content Value
                                            FROM dbo.mesPartFamilyDetail a
                                            JOIN dbo.luPartFamilyDetailDef f ON f.ID = a.PartFamilyDetailDefID
                                            JOIN dbo.luPartFamily b ON a.PartFamilyID = b.ID
                                            JOIN dbo.mesPart c ON c.PartFamilyID = b.ID
                                            WHERE c.ID = @PartId AND
                                            f.Description = @PartFamilyDetailDefName
                                            UNION ALL
                                            SELECT 'Part' Item, d.Content Value
                                            FROM dbo.mesPart c
                                            FULL JOIN dbo.mesPartDetail d ON d.PartID = c.ID
                                            FULL JOIN dbo.luPartDetailDef e ON e.ID = d.PartDetailDefID
                                            WHERE
                                                e.Description = @PartDetailDefName AND
                                                c.ID = @PartId", dynParams,null,I_DBTimeout,null);

			var tmpContext = scalageConfig.Where(x => x.Item == "Part")?.ToList();
			if (tmpContext is null or { Count : <= 0 })
			{
                tmpContext = scalageConfig.Where(x => x.Item == "PartFamily")?.ToList();
                if (tmpContext is null or { Count: <= 0 })
                    return ("70021", null);
            }

			string context = tmpContext[0].Value.Trim();
			context = context.Replace("，", ",").Replace("；", ";").ToUpper();
			//Base = 3.57; Unit = kg; UL = +0.05; LL = -0.04
			string[] contexts = context.Split(';');
			ScalageConfig sc = new ScalageConfig();
			foreach (var item in contexts)
			{
				string[] items = item.Split('=');
				if (items.Length != 2)
                    return ("70022", null);
				var proper = sc.GetType().GetProperty(items[0].Trim());
				if (proper.PropertyType.Name =="Double")
				{
                    sc.GetType().GetProperty(items[0].Trim()).SetValue(sc,  items[1].ToDouble());
                }
				else if (proper.PropertyType.Name == "String")
				{
                    sc.GetType().GetProperty(items[0].Trim()).SetValue(sc, items[1]);
                }
                
			}
			return ("1", sc);
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
			return ("70022", null);
        }
    }

    public async Task<(string, PartDetailValue)> GetMesPartAndPartFamilyDetail(int PartId, string PartDetailDefName, string PartFamilyDetailDefName = "")
    {
        try
        {
            if (string.IsNullOrEmpty(PartDetailDefName) && string.IsNullOrEmpty(PartFamilyDetailDefName))
                return ("70022", null);

            PartFamilyDetailDefName = string.IsNullOrEmpty(PartFamilyDetailDefName) ? PartDetailDefName : PartFamilyDetailDefName;
            PartDetailDefName = string.IsNullOrEmpty(PartDetailDefName) ? PartFamilyDetailDefName : PartDetailDefName;

            var dynParams = new DynamicParameters();
            dynParams.Add("PartId", PartId);
            dynParams.Add("PartDetailDefName", PartDetailDefName);
            dynParams.Add("PartFamilyDetailDefName", PartFamilyDetailDefName);

            var scalageConfig = await DapperConn.QueryAsync<ItemValues>(@"SELECT 'PartFamily'  Item,  a.Content Value
                                            FROM dbo.mesPartFamilyDetail a
                                            JOIN dbo.luPartFamilyDetailDef f ON f.ID = a.PartFamilyDetailDefID
                                            JOIN dbo.luPartFamily b ON a.PartFamilyID = b.ID
                                            JOIN dbo.mesPart c ON c.PartFamilyID = b.ID
                                            WHERE c.ID = @PartId AND
                                            f.Description = @PartFamilyDetailDefName
                                            UNION ALL
                                            SELECT 'Part' Item, d.Content Value
                                            FROM dbo.mesPart c
                                            FULL JOIN dbo.mesPartDetail d ON d.PartID = c.ID
                                            FULL JOIN dbo.luPartDetailDef e ON e.ID = d.PartDetailDefID
                                            WHERE
                                                e.Description = @PartDetailDefName AND
                                                c.ID = @PartId", dynParams, null, I_DBTimeout, null);

            PartDetailValue partDetailValue = new PartDetailValue();
            var tmpContext = scalageConfig.Where(x => x.Item == "Part")?.ToList();			
            if (tmpContext is null or { Count: <= 0 })
            {
                tmpContext = scalageConfig.Where(x => x.Item == "PartFamily")?.ToList();
                if (tmpContext is null or { Count: <= 0 })
                    return ("70021", null);
				else
				{
                    partDetailValue.IsPartSetup = false;
                    partDetailValue.DetailName = PartFamilyDetailDefName;
                }
            }
			else
			{
				partDetailValue.IsPartSetup = true;
				partDetailValue.DetailName = PartDetailDefName;
            }

            string context = tmpContext[0].Value.Trim();
            context = context.Replace("，", ",").Replace("；", ";").ToUpper();
			partDetailValue.DetailValue = context;
            return ("1", partDetailValue);
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
            return ("70022", null);
        }
    }
    /// <summary>
    /// 获取补打条码的相关信息
    /// </summary>
    /// <param name="MultipackSN"></param>
    /// <returns></returns>    
    public async Task<(string, List<ShipMentReprint>)> uspGetShipMentRePrintAsync(string MultipackSN)
    {
        try
        {
            ShipMentReprint shipMentReprint = new ShipMentReprint();
            string sql = $@"
	declare	@Fstatus INT,
	@strSNFormat      nvarchar(200) = '{MultipackSN}',			--MultipackSN
    @strOutput        nvarchar(200) 	
	BEGIN TRY
		DECLARE @ShipmentInterID	int,
				@LabelSCType		varchar(16),			--扫描类型为2时 打印的模板格式GS1/GS2 ；其类型空值
				@PalletSN			varchar(128)='',
				@PalletID			int

		SET @strOutput = 1
		
		IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat)
		BEGIN
			SET @strOutput='20012'
			SELECT @strOutput strOutput
			RETURN
		END

		SELECT TOP 1 @ShipmentInterID=ShipmentInterID FROM mesPackage WHERE  SerialNumber = @strSNFormat
		IF ISNULL(@ShipmentInterID,'')=''
		BEGIN
			SET @strOutput='20127'
			SELECT @strOutput strOutput
			RETURN
		END

		--打印模式2: 每扫描一次打印一张
		IF (SELECT COUNT(1) FROM(SELECT FKPONO FROM CO_WH_ShipmentEntryNew 
				WHERE FInterID=@ShipmentInterID GROUP BY FKPONO) WS)>1
		BEGIN
			IF NOT EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew A
						INNER JOIN mesPackage B ON A.FInterID=B.ShipmentInterID AND A.FDetailID=B.ShipmentDetailID
						AND A.FStatus in (2,3,4) and FInterID=@ShipmentInterID and b.Stage=3)
			BEGIN
				SET @strOutput='20045'
				SELECT @strOutput strOutput
				RETURN
			END
			SELECT @strOutput strOutput
			--判断扫描的条码类型(stage类型3栈板条码要转换为stage类型2中箱条码)
			IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat AND Stage=3)
			BEGIN
				SELECT TOP 1 A.SerialNumber,'' AS LabelSCType,@strSNFormat PalletSN,b.ID PalletID FROM mesPackage A 
				INNER JOIN mesPackage B ON A.ShipmentInterID=B.ShipmentInterID
				AND A.ShipmentParentID=B.ID AND B.SerialNumber=@strSNFormat 
				AND A.Stage=1 AND B.Stage=3
			END
			ELSE
			BEGIN
				SELECT @strSNFormat AS SerialNumber,'' AS LabelSCType,A.SerialNumber PalletSN,A.ID PalletID
				FROM mesPackage A INNER JOIN mesPackage B ON A.ShipmentInterID=B.ShipmentInterID AND A.ShipmentDetailID=B.ShipmentDetailID
				WHERE B.SerialNumber=@strSNFormat AND A.Stage=3
			END
		END
		--打印模式1: 扫描完毕打印
		ELSE
		BEGIN
			IF NOT EXISTS(SELECT 1 FROM CO_WH_ShipmentNew WHERE FInterID=@ShipmentInterID AND FStatus in (2,3,4))
			BEGIN
				SET @strOutput='20045'
				SELECT @strOutput strOutput
				RETURN
			END

			IF (SELECT COUNT(1) FROM(SELECT FMPNNO FROM CO_WH_ShipmentEntryNew 
				WHERE FInterID=@ShipmentInterID GROUP BY FMPNNO) WS)>1
			BEGIN
				SET @LabelSCType='2'
			END
			ELSE
			BEGIN
				SET @LabelSCType='1'
			END

			IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat AND Stage=3)
			BEGIN
				SELECT TOP 1 @PalletSN=SerialNumber,@PalletID=ID FROM mesPackage where SerialNumber=@strSNFormat 
			END
			ELSE
			BEGIN
				SELECT TOP 1 @PalletSN=SerialNumber,@PalletID=ID FROM mesPackage WHERE ShipmentInterID=@ShipmentInterID AND Stage=3
			END

			SELECT @strOutput strOutput
			SELECT B.SerialNumber,@LabelSCType AS LabelSCType,@PalletSN AS PalletSN,@PalletID AS PalletID 
			FROM CO_WH_ShipmentEntryNew A
			LEFT JOIN (SELECT * FROM mesPackage WHERE ID IN(SELECT MAX(ID) 
				FROM mesPackage WHERE ShipmentInterID=@ShipmentInterID AND Stage=1
				GROUP BY ShipmentDetailID)) B 
			ON A.FDetailID=B.ShipmentDetailID WHERE A.FInterID=@ShipmentInterID
		END

	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()
	END CATCH
";
			using (var output = await DapperConn.QueryMultipleAsync(sql, null, null, I_DBTimeout, null))
			{
                var outputStr = await output.ReadAsync<SqlOutputStr>();

                string errorCode = outputStr.ToList<SqlOutputStr>()[0].strOutput;

                if (errorCode != "1")
                    return (errorCode, null);
				var outputVal = await output.ReadAsync<ShipMentReprint>();
				return ("1", outputVal.ToList<ShipMentReprint>());
            }
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
            return ("70022", null);
        }
    }
    public async Task<(string, List<ShipMentDetailData>)> uspGetShipMentDetailAsync(string BillNO)
    {
        try
        {
            ShipMentDetailData shipMentData = new ShipMentDetailData();
            string sql = $@"declare	@Fstatus INT,
							@strSNFormat      nvarchar(200) = '{BillNO}',			--BillNo
							@strOutput        nvarchar(200) 
							BEGIN TRY
								DECLARE @FBillNO VARCHAR(200)
		
								SET @strOutput = 1

								IF NOT EXISTS(SELECT 1 FROM CO_WH_ShipmentNew WHERE FBillNO=@strSNFormat)
								BEGIN
									SET @strOutput = '20012'
									SELECT @strOutput strOutput
									RETURN
								END

								SELECT @strOutput strOutput
								SELECT ROW_NUMBER() OVER(ORDER BY (SELECT A.ShipmentTime)) ID,A.SerialNumber MultiPackSN FROM mesPackage A 
								INNER JOIN CO_WH_ShipmentEntryNew B ON A.ShipmentDetailID=B.FDetailID
								INNER JOIN CO_WH_ShipmentNew C ON B.FInterID=C.FInterID
								WHERE C.FBillNO=@strSNFormat and A.Stage=1

							END TRY
							BEGIN CATCH
								SELECT ERROR_MESSAGE()
							END CATCH
";

            using (var output = await DapperConn.QueryMultipleAsync(sql, null, null, I_DBTimeout, null))
            {
                var outputStr = await output.ReadAsync<SqlOutputStr>();
				if (!outputStr.Any())
                    return ("code exception ..", null);

                string errorCode = outputStr.ToList<SqlOutputStr>()[0].strOutput;
                if (string.IsNullOrEmpty(errorCode))
                    return ("exception null..", null);

                if (errorCode != "1")
                    return (errorCode, null);
                var outputVal = await output.ReadAsync<ShipMentDetailData>();
                return ("1", outputVal.ToList<ShipMentDetailData>());
            }
            //var output = Data_Table(sql);
            //if (output.Columns.Count == 1)
            //{
            //    return (output.Rows[0][0].ToString(), null);
            //}
            //else
            //{
            //    List<ShipMentDetailData> lsmd = output.DataTableToModels<ShipMentDetailData>(shipMentData);
            //    return ("1", lsmd);
            //}
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
            return ("exception..", null);
        }
    }

    public async Task<string> uspGetShipmentPrintTypeAsync(string BillNO)
    {
        try
        {
            string sql = $@"	DECLARE @strOutput nvarchar(200), @strSNFormat nvarchar(200) = '{BillNO}'
				IF (SELECT COUNT(1) FROM (SELECT FKPONO FROM CO_WH_ShipmentEntryNew A
				INNER JOIN CO_WH_ShipmentNew B ON A.FInterID=B.FInterID
				WHERE B.FBillNO=@strSNFormat GROUP BY A.FKPONO) WS )>1
				BEGIN
					SET @strOutput=2
				END
				ELSE
				BEGIN
					SET @strOutput=1
				END
				SELECT @strOutput strOutput
";

			return	(await DapperConn.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null)).ToString();
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
            return "exception..";
        }
    }
    public async Task<(string, List<ShipMentData>)> uspGetShipMentDataAsync(string BillNO, string xmlPOID, string xmlPartID, string xmlStation, string xmlExtraData)
    {
        try
        {
            string sql = $@"BEGIN
				declare	@Fstatus INT,
						@strSNFormat      nvarchar(200) = '{BillNO}',			--BillNo		
						@xmlProdOrder     nvarchar(2000) = '{xmlPOID}', 
						@xmlPart          nvarchar(2000) = '{xmlPartID}',  
						@xmlStation       nvarchar(2000) = '{xmlStation}', 
						@xmlExtraData     nvarchar(2000) = '{xmlExtraData}',
						@strSNbuf         nvarchar(200),
						@strOutput        NVARCHAR(200)
				BEGIN TRY
					DECLARE @FBillNO VARCHAR(200)
		
					SET @strOutput = 1

					IF NOT EXISTS(SELECT 1 FROM CO_WH_ShipmentNew WHERE FBillNO=@strSNFormat)
					BEGIN
						SET @strOutput = '20012'
						SELECT @strOutput strOutput
						RETURN
					END

					SELECT top 1 @Fstatus=FStatus FROM CO_WH_ShipmentNew WHERE FBillNO=@strSNFormat
					IF isnull(@Fstatus,-1)<>1
					BEGIN
						SET @strOutput = '20045'
						SELECT @strOutput strOutput
						RETURN
					END
		
					----Start verify shipping label by region from shipment data of SAP. 2022-4-19
					----API process enhancement
					declare @idoc				int
					--declare @PartID				varchar(64)
					--declare @StationID			varchar(64)
					declare @ProdOrderID				varchar(64)
					--参数解析
					--EXEC sp_xml_preparedocument @idoc output, @xmlPart
					--SELECT @PartID=PartId
					--FROM   OPENXML (@idoc,			'/Part',2)
					--		WITH (PartId  int		'@PartID')   
					--EXEC sp_xml_removedocument @idoc

					--EXEC sp_xml_preparedocument @idoc output, @xmlStation
					--SELECT @StationID=StationID
					--FROM   OPENXML (@idoc,			'/Station',2)
					--		WITH (StationID  int	'@StationID')   
					--EXEC sp_xml_removedocument @idoc
		
					exec sp_xml_preparedocument @idoc output, @xmlProdOrder  
					SELECT @ProdOrderID=ProdId  
					  FROM   OPENXML (@idoc,   '/ProdOrder',2)  
						WITH (ProdId  int  '@ProdOrderID')     
					exec sp_xml_removedocument @idoc 

					declare @shipmentRegion  varchar(100)
					declare @PORegion varchar(100)
					declare @shipmentCarrier varchar(100)
					select @shipmentRegion=isnull(Region,''), @shipmentCarrier=Carrier from CO_WH_ShipmentNew s where s.FBillNO=@strSNFormat
					select @PORegion=isnull(content,'') from mesProductionOrderDetail pod where pod.ProductionOrderID=@ProdOrderID and pod.ProductionOrderDetailDefID=(select id from luProductionOrderDetailDef where Description='ShipmentRegion')
					if isnull(@shipmentCarrier,'')='1060020795' 
					begin
					set @shipmentRegion='EMEIA_KN'
					end
					if isnull(@PORegion,'')=''
					begin
						SET @strOutput = '20070'
						SELECT @strOutput strOutput
						RETURN
					end
		
					if ( @shipmentRegion <> @PORegion )
					begin
					--select  *from sysLanguage where Description like '%order%'
						SET @strOutput = '20050'
						SELECT @strOutput strOutput
						RETURN
					end

					----End verify shipping label by region from shipment data of SAP

					SELECT @strOutput strOutput
					SELECT B.FDetailID,B.FLineItem,B.FKPONO,B.FCTN,isnull(B.FOutSN,0) FOutSN,B.FMPNNO FROM CO_WH_ShipmentNew A
					INNER JOIN CO_WH_ShipmentEntryNew B ON A.FInterID=B.FInterID
					WHERE A.FBillNO=@strSNFormat

				END TRY
				BEGIN CATCH
					SELECT ERROR_MESSAGE()
				END CATCH
			END
";
            using (var output = await DapperConn.QueryMultipleAsync(sql, null, null, I_DBTimeout, null))
            {
                var outputStr = await output.ReadAsync<SqlOutputStr>();

                string errorCode = outputStr.ToList<SqlOutputStr>()[0].strOutput;
				if (string.IsNullOrEmpty(errorCode))
					return ("exception null..", null);
                if (errorCode != "1")
                    return (errorCode, null);
                var outputVal = await output.ReadAsync<ShipMentData>();
                return ("1", outputVal.ToList<ShipMentData>());
            }

            //var output = Data_Table(sql);
            //if (output.Columns.Count == 1)
            //{
            //    return (output.Rows[0][0].ToString(), null);
            //}
            //else
            //{
            //    List<ShipMentData> lsmd = output.DataTableToModels<ShipMentData>(shipMentData);
            //    return ("1", lsmd);
            //}
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
            return ("exception..", null);
        }
    }

    public async Task<IEnumerable<mesPackage>> GetShipmentPalletSNAsync(string BillNO)
    {

        string sql = $@"SELECT  * FROM mesPackage WHERE ShipmentInterID in
                            (SELECT FInterID FROM CO_WH_ShipmentNew WHERE FBillNO='{BillNO}')
                            AND Stage=3
					";

        return await DapperConn.QueryAsync<mesPackage>(sql, null, null, I_DBTimeout, null);
    }
    public async Task<ShipmentDetailOutput> SetShipmentMultipackAsync(string BillNO,string MultipackSn)
    {
        try
        {
            ShipmentDetailOutput shipMentData = new ShipmentDetailOutput();
            string sql = $@"	DECLARE @CurrProductionOrderID	int,
								@ShipmentDetailID		int,
								@Stage					INT,
				
								@FBillNO                NVARCHAR(200),  
								@SerialNumber           NVARCHAR(200),

								@ShipmentParentID       int, 
								@OutSNCount             int,    

								@strOutput              NVARCHAR(200)";

            sql += "Set @FBillNO='" + BillNO + "'" + "\r\n" +
					"Set @SerialNumber='" + MultipackSn + "'" + "\r\n";

            sql += @"
			SET @strOutput = 1

			IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @SerialNumber  AND ISNULL(ShipmentDetailID,-1)<>-1)
			BEGIN
				SET @strOutput='20183'
				SELECT @ShipmentDetailID AS FDetailID,@strOutput AS OutResult
				RETURN
			END

			SELECT @CurrProductionOrderID=CurrProductionOrderID,@Stage=Stage FROM mesPackage WHERE SerialNumber = @SerialNumber 
			IF ISNULL(@CurrProductionOrderID,-1)=-1
			BEGIN
				SET @strOutput='20007'
				SELECT @ShipmentDetailID AS FDetailID,@strOutput AS OutResult
				RETURN
			END

			IF @Stage<>1
			BEGIN
				SET @strOutput='20176'
				SELECT @ShipmentDetailID AS FDetailID,@strOutput AS OutResult
				RETURN
			END

			SELECT @ShipmentDetailID=A.FDetailID FROM CO_WH_ShipmentEntryNew A
				INNER JOIN CO_WH_ShipmentNew B ON A.FInterID=B.FInterID WHERE B.FBillNO=@FBillNO AND EXISTS(
				SELECT * FROM (SELECT A.Content FROM mesProductionOrderDetail A
				INNER JOIN luProductionOrderDetailDef B ON A.ProductionOrderDetailDefID=B.ID
				WHERE B.Description='MPN' AND A.ProductionOrderID=@CurrProductionOrderID) WS WHERE WS.Content=A.FMPNNO AND A.FStatus in (0,1))
			IF ISNULL(@ShipmentDetailID,-1)=-1
			BEGIN
				SET @strOutput='20080'
				SELECT @ShipmentDetailID AS FDetailID,@strOutput AS OutResult
				RETURN
			END

			IF EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FDetailID=@ShipmentDetailID AND FStatus not in (0,1))
			BEGIN
				SET @strOutput='20045'
				SELECT @ShipmentDetailID AS FDetailID,@strOutput AS OutResult
				RETURN
			END

			IF EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FDetailID=@ShipmentDetailID AND FOutSN>=FCTN)
			BEGIN
				SET @strOutput='20086'
				SELECT @ShipmentDetailID AS FDetailID,@strOutput AS OutResult
				RETURN
			END
			SELECT @ShipmentDetailID AS FDetailID,@strOutput AS OutResult";

            return await DapperConn.QueryFirstAsync<ShipmentDetailOutput>(sql, null, null, I_DBTimeout, null);
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
            return null;
        }
    }

    public async Task<ShippingOutCount> GetIsOutCountCompleteAsync(string BillNO, string MultipackPalletSn, string ShipmentDetailID)
    {
        try
        {
            ShippingOutCount shipMentData = new ShippingOutCount();
            string S_Sql =
                    "DECLARE @FInterID INT" + "\r\n" +
                    "DECLARE @ShipmentParentID INT" + "\r\n" +

                    "SELECT @FInterID = FInterID FROM CO_WH_ShipmentNew WHERE FBillNO = '" + BillNO + "'" + "\r\n" +
                    "SELECT @ShipmentParentID = Id FROM mesPackage WHERE SerialNumber = '" + MultipackPalletSn + "'" + "\r\n" +

                    "SELECT* FROM" + "\r\n" +
                    " (SELECT FCTN FROM CO_WH_ShipmentEntryNew  WHERE FInterID = @FInterID AND FDetailID = '" + ShipmentDetailID + "')A," + "\r\n" +
                    "(SELECT COUNT(1) OutCount FROM mesPackage WHERE ShipmentParentID = @ShipmentParentID AND ShipmentDetailID = '" +
                        ShipmentDetailID + "')B";

            return await DapperConn.QueryFirstAsync<ShippingOutCount>(S_Sql, null, null, I_DBTimeout, null);
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
            return null;
        }
    }

    /// <summary>
    /// 改写 ufnGetBomPartInfoByStationType
	/// 
    /// </summary>
    /// <param name="ParentPartID"></param>
    /// <param name="StationTypeID"></param>
    /// <returns></returns>
    public async Task<List<BomPartInfo>> MESGetBomPartInfoCAsync(int ParentPartID, int StationTypeID)
    {
        string S_Sql =
            $@"	 DECLARE @ParentPartID nvarchar(64) = '{ParentPartID}',@StationTypeID	int = '{StationTypeID}'
			 DECLARE  @BomPartInfoTable TABLE  --输出的数据表
			 (
				 PartID int,
				 PartNumber nvarchar(max),
				 ScanType int,
				 Pattern nvarchar(max),
				 FieldName nvarchar(max)
			 )
	
			INSERT INTO @BomPartInfoTable(PartID,PartNumber,ScanType,Pattern,FieldName)
			SELECT ID,PartNumber,ScanType,SN_Pattern,FieldName FROM (
				SELECT A.ID,PartNumber,b.Content,c.Description FROM mesPart A
				INNER JOIN mesPartDetail B ON A.ID=B.PartID
				INNER JOIN luPartDetailDef C ON B.PartDetailDefID=C.ID
				WHERE A.ID=@ParentPartID) D 		
			PIVOT (MAX(Content) FOR Description IN (SN_Pattern,ScanType,FieldName)) S

			INSERT INTO @BomPartInfoTable(PartID,PartNumber,ScanType,Pattern,FieldName)
				SELECT ID,PartNumber,ScanType,CASE WHEN ScanType IN (2,7,8) THEN Batch_Pattern ELSE SN_Pattern END,FieldName FROM(
			SELECT ORDERID,ID,PartNumber,ScanType,SN_Pattern,Batch_Pattern,FieldName FROM (
				SELECT F.ID ORDERID, A.ID,PartNumber,b.Content,c.Description FROM mesPart A
				INNER JOIN mesProductStructure F ON A.ID=F.PartID AND F.Status = 1
				left JOIN mesPartDetail B ON A.ID=B.PartID
				left JOIN luPartDetailDef C ON B.PartDetailDefID=C.ID
				WHERE F.ParentPartID=@ParentPartID and StationTypeID=@StationTypeID) D 
			PIVOT (MAX(Content) FOR Description IN (SN_Pattern,Batch_Pattern,ScanType,FieldName)) S ) W ORDER BY ORDERID

			SELECT *,'' Barcode,0 AS IsMainSn  FROM @BomPartInfoTable
			DELETE @BomPartInfoTable";

        if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
        {

        }
        var v_Query = await DapperConn.QueryAsync<BomPartInfo>(S_Sql, null, null, I_DBTimeout, null);
        return v_Query.ToList();
    }
    public async Task<string> SetMesPackageShipmentAsync(string ShipmentDetailID, string SerialNumber, string Type)
    {
        string S_Sql = string.Empty;
        string ShipmentInterID = "";
        S_Sql = string.Format(@"select *  from CO_WH_ShipmentEntryNew where FDetailID={0}", ShipmentDetailID);
		var tmpShipmentEntryNew = await DapperConn.QueryFirstAsync<CO_WH_ShipmentEntryNew>(S_Sql, null, null, I_DBTimeout, null);
		if (tmpShipmentEntryNew is null or { FInterID: <= 0 })
			return "20197";

        ShipmentInterID = tmpShipmentEntryNew.FInterID.ToString();
        if (Type == "1")
        {
            S_Sql = string.Format(@"UPDATE mesPackage SET ShipmentInterID={0},ShipmentTime=GETDATE() WHERE SerialNumber='{1}'",
                ShipmentInterID, SerialNumber);
        }
        else if (Type == "2")
        {
            S_Sql = string.Format(@"UPDATE mesPackage SET ShipmentInterID={0},ShipmentTime=GETDATE(),ShipmentDetailID={2} WHERE SerialNumber='{1}'",
                ShipmentInterID, SerialNumber, ShipmentDetailID);
        }

		var affactCount = await DapperConn.ExecuteAsync(S_Sql, null, null, I_DBTimeout, null);
        return affactCount > 0 ? "1": "20121";
    }
	/// <summary>
	/// 获取打印数据
	/// </summary>
	/// <param name="MultipackPalletSN">出货栈板条码</param>
	/// <returns></returns>
    public async Task<(string,List<ShipmentMupltipack>)> uspGetShipMentPrintDataAsync(string MultipackPalletSN)
    {
        string S_Sql = string.Empty;
        S_Sql = $@"	declare	@Fstatus INT, @strOutput nvarchar(200),@strSNFormat nvarchar(200) = '{MultipackPalletSN}'		
				BEGIN TRY
					DECLARE @ShipmentInterID int
		
					SET @strOutput = '1'
					SELECT TOP 1 @ShipmentInterID=ShipmentInterID FROM mesPackage WHERE SerialNumber=@strSNFormat

					IF ISNULL(@ShipmentInterID,'')=''
					BEGIN
						SET @strOutput='20197'
						SELECT @strOutput strOutput
						RETURN
					END
					SELECT @strOutput strOutput
					SELECT B.SerialNumber FROM CO_WH_ShipmentEntryNew A
					LEFT JOIN (SELECT * FROM mesPackage WHERE ID IN(SELECT MAX(ID) 
						FROM mesPackage WHERE ShipmentInterID=@ShipmentInterID AND Stage=1
						GROUP BY ShipmentDetailID)) B 
					ON A.FDetailID=B.ShipmentDetailID WHERE A.FInterID=@ShipmentInterID
					ORDER BY B.ShipmentDetailID,B.ID

				END TRY
				BEGIN CATCH
					SELECT ERROR_MESSAGE()
				END CATCH
				";

        using (var output = await DapperConn.QueryMultipleAsync(S_Sql, null, null, I_DBTimeout, null))
        {
            var outputStr = await output.ReadAsync<SqlOutputStr>();

            string errorCode = outputStr.ToList<SqlOutputStr>()[0].strOutput;

            if (errorCode != "1")
                return (errorCode, null);
            var outputVal = await output.ReadAsync<ShipmentMupltipack>();
            return ("1", outputVal.ToList<ShipmentMupltipack>());
        }
    }
    public async Task<MesLabelData> GetMesLabelDataAsync(string LabelName)
    {
        string S_Sql = string.Empty;
        S_Sql = string.Format(@"SELECT NAME+','+SourcePath+','+ CAST(OutputType AS VARCHAR)+','+TargetPath+','+CAST(PageCapacity AS varchar)  AS LablePath FROM mesLabel WHERE NAME ='{0}'", LabelName);

        return await DapperConn.QueryFirstAsync<MesLabelData>(S_Sql, null, null, I_DBTimeout, null);
    }

    public async Task<KitBoxCheckOutput> uspReplaceShipmentPalletAsync(string S_BillNo_Old, string S_BillNo_New)
    {
        string S_Sql = string.Empty;
        S_Sql = $@"declare	@prodID				int,
			@partID				int,
			@stationID			int,
			@routeID			int,
			@lineID				int,
			@stationTypeID		int,
			@UnitStateID		int,
			@EmployeeId			int,
			@PackageID			int,
			@stage				int,
			@idoc				INT,
			@strOutput        nvarchar(200),
			@strSNFormat      nvarchar(200) = '{S_BillNo_Old}',
			@strSNbuf         nvarchar(200) = '{S_BillNo_New}',
			@BoxSN			  NVARCHAR(200)

	
		BEGIN TRY
			DECLARE @CurrProductionOrderID	int,
					@CurrShipmentDetailID		int,
					--@ShipmentDetailID2	int,
					@CurrOutSN int,
					@CurrShipmentPalletID int,
					@CurrShipmentEnterID int,
					@CurrShipmentEnterID2 int
			DECLARE @TotalEntryOutCount INT,@TotalEntryCount INT, @TotalCount INT,@OutTotalFGQty INT,@TotalFGQty INT,@currentTotalFGQty INT

			SET @strOutput = 1
			--查询旧Bill NO是否存在
			IF NOT EXISTS(SELECT 1 FROM dbo.CO_WH_ShipmentNew WHERE FBillNO = @strSNbuf AND FStatus IN (3,4))
			BEGIN
				PRINT '旧Bill No不存在或状态不为已出货状态'
				SET @strOutput = 70040
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN 
			END
			SELECT ROW_NUMBER() OVER(ORDER BY FInterID) AS rowIndex, * INTO #tmpCO_WH_ShipmentNew FROM dbo.CO_WH_ShipmentNew WHERE FBillNO = @strSNbuf AND FStatus IN (3,4)
			IF	NOT EXISTS(SELECT 1 FROM dbo.CO_WH_ShipmentEntryNew 
						WHERE FInterID IN (SELECT FInterID FROM dbo.CO_WH_ShipmentNew WHERE FBillNO = @strSNbuf AND FStatus IN (3,4)))
			BEGIN
				PRINT '旧Bill No不存在对应的MPN信息'
				SET @strOutput = 70041
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN    
			END

			SELECT ROW_NUMBER() OVER(ORDER BY FInterID,FDetailID) AS rowIndex, * 
			INTO #tmpCO_WH_ShipmentEntryNew FROM dbo.CO_WH_ShipmentEntryNew 
			WHERE FInterID IN (SELECT FInterID FROM dbo.CO_WH_ShipmentNew WHERE FBillNO = @strSNbuf AND FStatus IN (3,4))

			IF (SELECT COUNT(1) FROM ( SELECT FKPONO FROM #tmpCO_WH_ShipmentEntryNew GROUP BY FKPONO)a) > 1
			BEGIN
				PRINT '旧BillNo, 存在多个PO'
				SET @strOutput = 70041
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			END

			SELECT @TotalEntryOutCount = SUM(ISNULL(FOutSN,0)), @TotalEntryCount= SUM(ISNULL(FCTN,0)),@OutTotalFGQty = SUM(ISNULL(FQTY,0)) 
			FROM #tmpCO_WH_ShipmentEntryNew
			SELECT @TotalCount = ISNULL(FCTN,0) FROM #tmpCO_WH_ShipmentNew
			IF	@TotalEntryOutCount<> @TotalEntryCount OR @TotalEntryCount <> @TotalCount
			BEGIN
				PRINT '旧Bill No对应的总箱数不匹配'
				SET @strOutput = 70042
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			END
		
			--查询旧Bill No是否有关联mespack
			IF	NOT EXISTS(SELECT 1 FROM dbo.mesPackage a JOIN #tmpCO_WH_ShipmentEntryNew b ON a.ShipmentInterID = b.FInterID)
			BEGIN
				PRINT '旧Bill No不存在对应的包装信息'
				SET @strOutput = 70043
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			END
			--将关联的mesPack信息插入备份表中
			SELECT a.* 
			INTO #tmpMesPackage
			FROM dbo.mesPackage a
			WHERE a.ShipmentInterID IN (SELECT DISTINCT FInterID FROM #tmpCO_WH_ShipmentEntryNew ) 
		
			SELECT *
			INTO #tmpMesPackageNoParent 
			FROM #tmpMesPackage
			WHERE ShipmentParentID IS NOT NULL

			SELECT @TotalFGQty = COUNT(c.ID)
			FROM #tmpCO_WH_ShipmentEntryNew a
			JOIN #tmpMesPackageNoParent b ON a.FInterID = b.ShipmentInterID AND a.FDetailID = b.ShipmentDetailID
			JOIN dbo.mesUnitDetail c ON c.InmostPackageID = b.ID
		
			SELECT @currentTotalFGQty = SUM(ISNULL(b.CurrentCount,0))
			FROM #tmpCO_WH_ShipmentEntryNew a
			JOIN #tmpMesPackageNoParent b ON a.FInterID = b.ShipmentInterID AND a.FDetailID = b.ShipmentDetailID
			IF	@TotalFGQty<> @currentTotalFGQty OR @currentTotalFGQty <> @OutTotalFGQty
			BEGIN
				PRINT '旧Bill No对应的产品条码数不匹配'
				SET @strOutput = 70044
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			END

			--查询新的Bill No中是否已存在绑定记录
			IF not EXISTS(SELECT 1 FROM dbo.CO_WH_ShipmentNew WHERE FBillNO = @strSNFormat AND FStatus = 1)
			BEGIN
				PRINT '新Bill No不存在，或者状态不对'
				SET @strOutput = 70045
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			END

			SELECT ROW_NUMBER() OVER(ORDER BY FInterID) AS rowIndex, * INTO #tmpNewCO_WH_ShipmentNew 
			FROM dbo.CO_WH_ShipmentNew WHERE FBillNO = @strSNFormat AND FStatus = 1
			SELECT ROW_NUMBER() OVER(ORDER BY a.FInterID,a.FDetailID) AS rowIndex, a.* 
			INTO #tmpNewCO_WH_ShipmentEntryNew FROM dbo.CO_WH_ShipmentEntryNew a
			JOIN #tmpNewCO_WH_ShipmentNew  b ON b.FInterID = a.FInterID

			IF	(SELECT COUNT(rowIndex) FROM #tmpCO_WH_ShipmentEntryNew) <> (SELECT COUNT(rowIndex) FROM #tmpNewCO_WH_ShipmentEntryNew)
			BEGIN
				PRINT '新旧Bill No对应的MPN行数不匹配'
				SET @strOutput = 70046
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			END

			DECLARE @mFEntryID INT,@mFInterID INT, @mFDetail INT, @mFMPNNO VARCHAR(50), @mFCTN INT , @mRes BIT = 1
			DECLARE shipMentCur CURSOR FOR SELECT FEntryID,FMPNNO,FCTN FROM #tmpCO_WH_ShipmentEntryNew
			OPEN shipMentCur
			FETCH NEXT FROM shipMentCur INTO @mFEntryID,@mFMPNNO,@mFCTN
			WHILE(@@FETCH_STATUS = 0)
			BEGIN
				IF NOT EXISTS(SELECT 1 FROM #tmpNewCO_WH_ShipmentEntryNew WHERE 
					FMPNNO = @mFMPNNO AND FCTN = @mFCTN)
				BEGIN
					SET	 @mRes = 0
				END
				FETCH NEXT FROM shipMentCur INTO @mFEntryID,@mFMPNNO,@mFCTN
			END
			CLOSE shipMentCur
			DEALLOCATE shipMentCur
			IF @mRes = 0
			BEGIN
				PRINT '新旧Bill No对应的MPN和数量不匹配'
				SET @strOutput = 70047
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			END

			declare @PackageDetailDefID_ShipmentDetailID int,@PackageDetailDefID_ShipmentInterID int
			select  @PackageDetailDefID_ShipmentDetailID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentDetailID'
			select  @PackageDetailDefID_ShipmentInterID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentInterID'
			if ( @PackageDetailDefID_ShipmentDetailID=0 or @PackageDetailDefID_ShipmentInterID=0 )
			begin
				SET @strOutput='20200'
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			end

			BEGIN TRANSACTION t1
			--插入备份
			INSERT INTO dbo.mesPackageRecord
			SELECT ID, SerialNumber, StationID, EmployeeID, CreationTime, CurrentCount, StatusID, LastUpdate, ParentID, ShipmentParentID, Stage, CurrProductionOrderID, CurrPartID, ShipmentInterID, ShipmentDetailID, ShipmentTime,BOXID,PALLETID, KNSN
			FROM #tmpMesPackage       

			--重新指定innerID,DetilID

			DECLARE @mNewFInterID INT, @mNewFDetail INT
			DECLARE updateMesPackageCur CURSOR FOR SELECT FInterID,FDetailID, FMPNNO,FCTN FROM #tmpCO_WH_ShipmentEntryNew
			OPEN updateMesPackageCur
			FETCH NEXT FROM updateMesPackageCur INTO @mFInterID,@mFDetail,@mFMPNNO,@mFCTN
			WHILE(@@FETCH_STATUS = 0)
			BEGIN
			
				IF NOT EXISTS(SELECT 1 FROM #tmpNewCO_WH_ShipmentEntryNew WHERE FMPNNO = @mFMPNNO AND FCTN = @mFCTN AND FOutSN = 0)
				BEGIN
					SET @mRes = 0
					BREAK
				END
				SELECT @mNewFInterID = FInterID, @mNewFDetail = FDetailID FROM #tmpNewCO_WH_ShipmentEntryNew 
				WHERE FMPNNO = @mFMPNNO AND FCTN = @mFCTN AND FOutSN = 0

 
				UPDATE a SET a.ShipmentInterID = @mNewFInterID, a.ShipmentDetailID = @mNewFDetail,a.ShipmentTime = GETDATE()
				FROM dbo.mesPackage a
				JOIN #tmpMesPackageNoParent b ON b.ID = a.ID
				WHERE b.ShipmentInterID = @mFInterID AND b.ShipmentDetailID = @mFDetail

				UPDATE dbo.CO_WH_ShipmentEntryNew SET FOutSN = @mFCTN
				WHERE FInterID = @mNewFInterID AND FDetailID = @mNewFDetail

				insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
				select id,@PackageDetailDefID_ShipmentDetailID,ShipmentDetailID from #tmpMesPackage 
				WHERE ShipmentDetailID = @mFDetail AND ShipmentInterID = @mFInterID

				insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
				SELECT id,@PackageDetailDefID_ShipmentInterID, ShipmentInterID from #tmpMesPackage
				WHERE ShipmentDetailID = @mFDetail AND ShipmentInterID = @mFInterID

				FETCH NEXT FROM updateMesPackageCur INTO @mFInterID,@mFDetail,@mFMPNNO,@mFCTN
			END
			CLOSE updateMesPackageCur
			DEALLOCATE updateMesPackageCur
		
			IF	@mRes = 0
			BEGIN
				PRINT '新Bill No对应的出货产出数量不为0'
				SET @strOutput = 70048
				SELECT @strOutput strOutput,@BoxSN BoxSN
				RETURN
			END

			insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
			SELECT id,@PackageDetailDefID_ShipmentInterID, ShipmentInterID from #tmpMesPackage
			WHERE ShipmentDetailID IS NULL AND ShipmentInterID = @mFInterID


			UPDATE a SET a.ShipmentInterID = @mNewFInterID, a.ShipmentTime = GETDATE()
			FROM dbo.mesPackage a
			JOIN #tmpMesPackage b ON b.ID = a.ID
			WHERE b.ShipmentParentID IS NULL

			UPDATE dbo.CO_WH_ShipmentNew SET FStatus = 1 WHERE FInterID = @mFInterID
			UPDATE dbo.CO_WH_ShipmentEntryNew SET FStatus = 0, FOutSN = 0 WHERE FInterID = @mFInterID
			UPDATE dbo.CO_WH_ShipmentNew SET FStatus = 2 WHERE FInterID = @mNewFInterID	
			UPDATE dbo.CO_WH_ShipmentEntryNew SET FStatus = 2 WHERE FInterID = @mNewFInterID	

			COMMIT TRANSACTION t1
			SET @strOutput = 1
		
			SELECT TOP 1 @BoxSN = SerialNumber FROM #tmpMesPackageNoParent
			DROP TABLE #tmpMesPackage
			DROP TABLE #tmpNewCO_WH_ShipmentEntryNew
			DROP TABLE #tmpCO_WH_ShipmentEntryNew
			DROP TABLE #tmpCO_WH_ShipmentNew
			DROP TABLE #tmpMesPackageNoParent
			DROP TABLE #tmpNewCO_WH_ShipmentNew
			SELECT @strOutput strOutput,@BoxSN BoxSN
		END TRY
		BEGIN CATCH
			SET @strOutput = -1
			IF @@TRANCOUNT >0 
			BEGIN
				ROLLBACK TRANSACTION
			END
			SELECT ERROR_MESSAGE() strOutput,@BoxSN BoxSN
		END CATCH
		";
		return await DapperConn.QueryFirstOrDefaultAsync<KitBoxCheckOutput>(S_Sql, null, null, I_DBTimeout, null);
    }
    public async Task<SqlOutputStr> uspMoveShipmentMultipackAsync(string BillNO, string MultipackSN, string xmlProdOrder, string xmlPartID, string xmlStation, string xmlExtraData)
    {
        string S_Sql = string.Empty;
        S_Sql = $@"		
			BEGIN
				DECLARE 	@strSNFormat      nvarchar(200) = '{MultipackSN}',			--MultipackSN		
							@xmlProdOrder     nvarchar(2000) = '{xmlProdOrder}', 
							@xmlPart          nvarchar(2000) = '{xmlPartID}',  
							@xmlStation       nvarchar(2000) = '{xmlStation}', 
							@xmlExtraData     nvarchar(2000) = '{xmlExtraData}',
							@strSNbuf         nvarchar(200) = '{BillNO}',			--BillNo
							@strOutput        nvarchar(200) 		--0：全部完成 1：正常执行 其他：错误代码 

				declare	@prodID				int,
						@partID				int,
						@stationID			int,
						@routeID			int,
						@lineID				int,
						@stationTypeID		int,
						@UnitStateID		int,
						@EmployeeId			int,
						@PackageID			int,
						@stage				int,
						@idoc				int


				--读取xmlPart参数值
		

					exec sp_xml_preparedocument @idoc output, @xmlStation
					SELECT @stationID=StationId
					FROM   OPENXML (@idoc, '/Station',2)
							WITH (StationId		  int			'@StationId')   
					exec sp_xml_removedocument @idoc
					IF isnull(@stationID,'') = ''
					BEGIN
						SET @strOutput= '20077'
						SELECT @strOutput strOutput
						RETURN
					END

					exec sp_xml_preparedocument @idoc output, @xmlExtraData
					SELECT @EmployeeId=EmployeeId
					FROM   OPENXML (@idoc, '/ExtraData',2)
							WITH (EmployeeId		  int		'@EmployeeId')   
					exec sp_xml_removedocument @idoc
					IF isnull(@stationID,'') = ''
					BEGIN
						SET @strOutput= '20077'
						SELECT @strOutput strOutput
						RETURN
					END
	
				-- select  *from syslanguage where description like '%state%'
				-- select  * from sysLanguage where code='20176'--20204
				BEGIN TRY
					DECLARE @CurrProductionOrderID	int,
							@CurrShipmentDetailID		int,
							--@ShipmentDetailID2	int,
							@CurrOutSN int,
							@CurrShipmentPalletID int,
							@CurrShipmentEnterID int,
							@CurrShipmentEnterID2 int
				

					SET @strOutput = 1
		
					-- not exists BoxSN
					IF not EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat and stage=1)
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- BoxSN no shipmentPallet relationship
					IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat and Stage=1  AND ISNULL(ShipmentDetailID,0)=0)
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- shipPallet status is wrong
					IF not EXISTS(SELECT 1 FROM CO_WH_ShipmentNew WHERE FBillNO = @strSNbuf and FStatus  in (1,2,3,4) )
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- not correct BillNo & BoxSN relationship
					select @CurrShipmentEnterID2=isnull(s.FInterID,0) from CO_WH_ShipmentNew s 
					join CO_WH_ShipmentEntryNew se on s.FInterID=se.FInterID where s.FBillNO=@strSNbuf
		
					select @CurrShipmentEnterID=isnull(ShipmentInterID,0),@CurrShipmentDetailID=isnull(ShipmentDetailID,0),@CurrShipmentPalletID=isnull(ShipmentParentID,0)
					from mesPackage  where SerialNumber=@strSNFormat

					if (@CurrShipmentEnterID<>@CurrShipmentEnterID2) or (@CurrShipmentEnterID=0) or (@CurrShipmentEnterID=0) or(@CurrShipmentPalletID=0)
					begin
						SET @strOutput='20183'
						SELECT @strOutput strOutput
						RETURN
					end
					 if exists(select 1 from CO_WH_ShipmentEntryNew where FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID and isnull(FOutSN,0)<=0)
					 begin
						 SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					 end
		
					declare @PackageDetailDefID_ShipmentDetailID int,@PackageDetailDefID_ShipmentEnterID int,@PackageDetailDefID_ShipmentParentID int
					select  @PackageDetailDefID_ShipmentDetailID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentDetailID'
					select  @PackageDetailDefID_ShipmentEnterID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentInterID'
					select  @PackageDetailDefID_ShipmentParentID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentParentID'

					if ( @PackageDetailDefID_ShipmentDetailID=0 or @PackageDetailDefID_ShipmentEnterID=0 or @PackageDetailDefID_ShipmentEnterID=0 )
					begin
						set @strOutput='20200'
						SELECT @strOutput strOutput
						RETURN
					end

					------base on Sn to find Route and unitstateID for update traget
					declare @CurrStateID int,@ProductionOrderID int,@unitId int,@StationID_Pre int
					CREATE TABLE #TempRoute
					(	
						ID				int,
						UnitStateID		int
					)
					select  top 1 @unitId=ud.UnitID,@CurrStateID=UnitStateID,@PartID=u.PartID,@LineID=u.LineID,@ProductionOrderID=u.ProductionOrderID,@StationID_Pre=u.StationID 
					from mesUnitDetail ud
					join mesUnit u on ud.UnitID=u.ID
					join mesPackage p on ud.InmostPackageID=p.id where p.SerialNumber=@strSNFormat and Stage=1
			
						--SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
					select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID

					select top 1 @routeID = RouteID
					from mesRouteMap
					where
						(LineID = @LineID or LineID is NULL OR LineID=0) AND
						(PartID = @PartID or PartID is NULL OR PartID=0) AND
						(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
						(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
					order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc

						IF ISNULL(@routeID,'')=''
						BEGIN
							SET @strOutput='20195'
							SELECT @strOutput strOutput
							RETURN
						END
			
						--根据route类型获取上一站UnitstateID
						IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
						BEGIN
							IF(SELECT COUNT(1) FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
								WHERE RouteID=@routeID)>1
							BEGIN
								SET @strOutput='20203'
								SELECT @strOutput strOutput
								RETURN
							END

							SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
							WHERE RouteID=@routeID
						END
						ELSE
						BEGIN
							INSERT INTO #TempRoute(ID,UnitStateID)
							SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
							FROM mesRouteDetail where RouteID=@routeID

							SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
								(SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
						END

						IF ISNULL(@UnitStateID,'')=''
						BEGIN
							SET @strOutput='20131'
							SELECT @strOutput strOutput
							RETURN
						END

					------end unitstateID for update unit traget

					--declare @AfterMovedShippingPalletUnitStateID varchar(100)
					--select @AfterMovedShippingPalletUnitStateID=isnull(value,'') from mesStationConfigSetting where StationID=@stationID and name='AfterMovedShippingPalletUnitStateID'
					--if @AfterMovedShippingPalletUnitStateID='' 
					--begin
					--set @strOutput='20199'
					----select  *from sysLanguage where Description like '%parameter%'
					--return
					--end

					BEGIN TRANSACTION Task

		
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentDetailID,ShipmentDetailID from mesPackage where SerialNumber=@strSNFormat and Stage=1
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentEnterID,ShipmentInterID from mesPackage where SerialNumber=@strSNFormat and Stage=1
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentParentID,ShipmentParentID from mesPackage where SerialNumber=@strSNFormat and Stage=1

						-- update FG unitState
						declare @StationId_History    int, @LineId_History int,@PODID_History int, @PartID_History int
						DECLARE @UnitState NVARCHAR(200)
						SELECT @UnitState=[Description] FROM mesUnitState WHERE ID=@UnitStateID

						IF NOT EXISTS (SELECT TOP 1 * FROM mesHistory WHERE UnitID= @unitId AND UnitStateID=@UnitStateID ) 
						BEGIN
							--SET @strOutput='ERROR:没有过站记录('+@UnitState+')  不允许回退没有扫描过的站。'
							--SET @strOutput='ERROR: can t found history of the '+ @UnitState
							SET @strOutput= '70061'
							SELECT @strOutput strOutput
							RETURN
						END 
						SELECT TOP 1  @StationId_History=StationID,@PODID_History=ProductionOrderID,@PartID_History=PartID 
						FROM mesHistory WHERE UnitID= @unitId AND UnitStateID= @UnitStateID ORDER BY ID

						select @LineId_History=LineID from mesStation where id=@StationId_History
						update u set u.UnitStateID=@UnitStateID,u.StationID=@StationId_History,u.LineID=@LineId_History,u.EmployeeID=@EmployeeID,u.LastUpdate=GETDATE()
						from mesUnit u join mesUnitDetail ud on u.ID=ud.UnitID 
						join mesPackage p on p.id=ud.InmostPackageID
						where p.SerialNumber=@strSNFormat and Stage=1

			
						UPDATE A SET A.ShipmentDetailID =null,A.ShipmentTime=null,ShipmentInterID=null,ShipmentParentID=null,LastUpdate=getdate()
						FROM mesPackage A WHERE SerialNumber = @strSNFormat and Stage=1
						
						UPDATE CO_WH_ShipmentEntryNew SET FOutSN=ISNULL(FOutSN,0)-1 WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID --and FStatus in (0,1,2)
						-- 2022-05-05 增加  StatusID = 0
						UPDATE mesPackage SET CurrentCount=CurrentCount-1,StatusID = 0  WHERE ID=@CurrShipmentPalletID
			

						IF  EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID AND FOutSN <> FCTN and FOutSN<>0)
						BEGIN
							UPDATE CO_WH_ShipmentEntryNew SET FStatus=1 WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID
						end 
						IF  EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID AND  FOutSN=0)
						BEGIN
							UPDATE CO_WH_ShipmentEntryNew SET FStatus=0 WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID
						end
			
						IF  EXISTS( SELECT 1 FROM CO_WH_ShipmentEntryNew A
								INNER JOIN CO_WH_ShipmentNew B ON A.FInterID=B.FInterID
								WHERE B.FBillNO=@strSNbuf AND A.FOutSN<>a.FCTN )
							BEGIN
								UPDATE CO_WH_ShipmentNew SET FStatus=1 WHERE FBillNO=@strSNbuf
							END

						IF not exists (select 1 from mespackage where ShipmentParentID=@CurrShipmentPalletID )
							BEGIN
							insert into mesPackageHistory
							select @CurrShipmentPalletID,10,@stationID,@EmployeeId,getdate()
							UPDATE mesPackage SET ShipmentInterID=null, ShipmentTime=null WHERE ID=@CurrShipmentPalletID
							END

						INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
						SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesUnitDetail A 
							INNER JOIN mesUnit B ON A.UnitID=B.ID
							inner join mesPackage P on P.ID=A.InmostPackageID
						WHERE P.SerialNumber=@strSNFormat and Stage=1

						insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,[Time])
						select id,9,@stationID,@EmployeeId,getdate() from mesPackage where SerialNumber=@strSNFormat and Stage=1
						SET @strOutput=1
					COMMIT TRANSACTION Task
					SELECT @strOutput strOutput
				END TRY
				BEGIN CATCH
					IF @@TRANCOUNT >0 
					BEGIN
						ROLLBACK TRANSACTION
					END
					SELECT ERROR_MESSAGE() strOutput
				END CATCH
			END
			";
        return await DapperConn.QueryFirstAsync<SqlOutputStr>(S_Sql, null, null, I_DBTimeout, null);
    }
    public async Task<SqlOutputStr> uspMoveShipmentMultipackAsync_P(string BillNO, string MultipackSN, string xmlProdOrder, string xmlPartID, string xmlStation, string xmlExtraData)
    {
        string S_Sql = string.Empty;
        S_Sql = $@"		
			BEGIN
				DECLARE 	@strSNFormat      nvarchar(200) = '{MultipackSN}',			--MultipackSN		
							@xmlProdOrder     nvarchar(2000) = '{xmlProdOrder}', 
							@xmlPart          nvarchar(2000) = '{xmlPartID}',  
							@xmlStation       nvarchar(2000) = '{xmlStation}', 
							@xmlExtraData     nvarchar(2000) = '{xmlExtraData}',
							@strSNbuf         nvarchar(200) = '{BillNO}',			--BillNo
							@strOutput        nvarchar(200) 		--0：全部完成 1：正常执行 其他：错误代码 

				declare	@prodID				int,
						@partID				int,
						@stationID			int,
						@routeID			int,
						@lineID				int,
						@stationTypeID		int,
						@UnitStateID		int,
						@EmployeeId			int,
						@PackageID			int,
						@stage				int,
						@idoc				int


				--读取xmlPart参数值
		

					exec sp_xml_preparedocument @idoc output, @xmlStation
					SELECT @stationID=StationId
					FROM   OPENXML (@idoc, '/Station',2)
							WITH (StationId		  int			'@StationId')   
					exec sp_xml_removedocument @idoc
					IF isnull(@stationID,'') = ''
					BEGIN
						SET @strOutput= '20077'
						SELECT @strOutput strOutput
						RETURN
					END

					exec sp_xml_preparedocument @idoc output, @xmlExtraData
					SELECT @EmployeeId=EmployeeId
					FROM   OPENXML (@idoc, '/ExtraData',2)
							WITH (EmployeeId		  int		'@EmployeeId')   
					exec sp_xml_removedocument @idoc
					IF isnull(@stationID,'') = ''
					BEGIN
						SET @strOutput= '20077'
						SELECT @strOutput strOutput
						RETURN
					END
	
				-- select  *from syslanguage where description like '%state%'
				-- select  * from sysLanguage where code='20176'--20204
				BEGIN TRY
					DECLARE @CurrProductionOrderID	int,
							@CurrShipmentDetailID		int,
							--@ShipmentDetailID2	int,
							@CurrOutSN int,
							@CurrShipmentPalletID int,
							@CurrShipmentEnterID int,
							@CurrShipmentEnterID2 int
				

					SET @strOutput = 1
		
					-- not exists BoxSN
					IF not EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat and stage=1)
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- BoxSN no shipmentPallet relationship
					IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat and Stage=1  AND ISNULL(ShipmentDetailID,0)=0)
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- shipPallet status is wrong
					IF not EXISTS(SELECT 1 FROM CO_WH_ShipmentNew WHERE FBillNO = @strSNbuf and FStatus  in (1,2,3,4) )
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- not correct BillNo & BoxSN relationship
					select @CurrShipmentEnterID2=isnull(s.FInterID,0) from CO_WH_ShipmentNew s 
					join CO_WH_ShipmentEntryNew se on s.FInterID=se.FInterID where s.FBillNO=@strSNbuf
		
					select @CurrShipmentEnterID=isnull(ShipmentInterID,0),@CurrShipmentDetailID=isnull(ShipmentDetailID,0),@CurrShipmentPalletID=isnull(ShipmentParentID,0)
					from mesPackage  where SerialNumber=@strSNFormat

					if (@CurrShipmentEnterID<>@CurrShipmentEnterID2) or (@CurrShipmentEnterID=0) or (@CurrShipmentEnterID=0) or(@CurrShipmentPalletID=0)
					begin
						SET @strOutput='20183'
						SELECT @strOutput strOutput
						RETURN
					end
					 if exists(select 1 from CO_WH_ShipmentEntryNew where FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID and isnull(FOutSN,0)<=0)
					 begin
						 SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					 end
		
					declare @PackageDetailDefID_ShipmentDetailID int,@PackageDetailDefID_ShipmentEnterID int,@PackageDetailDefID_ShipmentParentID int
					select  @PackageDetailDefID_ShipmentDetailID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentDetailID'
					select  @PackageDetailDefID_ShipmentEnterID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentInterID'
					select  @PackageDetailDefID_ShipmentParentID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentParentID'

					if ( @PackageDetailDefID_ShipmentDetailID=0 or @PackageDetailDefID_ShipmentEnterID=0 or @PackageDetailDefID_ShipmentEnterID=0 )
					begin
						set @strOutput='20200'
						SELECT @strOutput strOutput
						RETURN
					end

					------base on Sn to find Route and unitstateID for update traget
					declare @CurrStateID int,@ProductionOrderID int,@unitId int,@StationID_Pre int
					CREATE TABLE #TempRoute
					(	
						ID				int,
						UnitStateID		int
					)
					select  top 1 @unitId=ud.UnitID,@CurrStateID=UnitStateID,@PartID=u.PartID,@LineID=u.LineID,@ProductionOrderID=u.ProductionOrderID,@StationID_Pre=u.StationID 
					from mesUnitDetail ud
					join mesUnit u on ud.UnitID=u.ID
					join mesPackage p on ud.InmostPackageID=p.id where p.SerialNumber=@strSNFormat and Stage=1
			
						SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
						IF ISNULL(@routeID,'')=''
						BEGIN
							SET @strOutput='20195'
							SELECT @strOutput strOutput
							RETURN
						END
			
						--根据route类型获取上一站UnitstateID
						IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
						BEGIN
							IF(SELECT COUNT(1) FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
								WHERE RouteID=@routeID)>1
							BEGIN
								SET @strOutput='20203'
								SELECT @strOutput strOutput
								RETURN
							END

							SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
							WHERE RouteID=@routeID
						END
						ELSE
						BEGIN
							INSERT INTO #TempRoute(ID,UnitStateID)
							SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
							FROM mesRouteDetail where RouteID=@routeID

							SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
								(SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
						END

						IF ISNULL(@UnitStateID,'')=''
						BEGIN
							SET @strOutput='20131'
							SELECT @strOutput strOutput
							RETURN
						END

					------end unitstateID for update unit traget

					--declare @AfterMovedShippingPalletUnitStateID varchar(100)
					--select @AfterMovedShippingPalletUnitStateID=isnull(value,'') from mesStationConfigSetting where StationID=@stationID and name='AfterMovedShippingPalletUnitStateID'
					--if @AfterMovedShippingPalletUnitStateID='' 
					--begin
					--set @strOutput='20199'
					----select  *from sysLanguage where Description like '%parameter%'
					--return
					--end

					BEGIN TRANSACTION Task

		
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentDetailID,ShipmentDetailID from mesPackage where SerialNumber=@strSNFormat and Stage=1
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentEnterID,ShipmentInterID from mesPackage where SerialNumber=@strSNFormat and Stage=1
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentParentID,ShipmentParentID from mesPackage where SerialNumber=@strSNFormat and Stage=1

						-- update FG unitState
						declare @StationId_History    int, @LineId_History int,@PODID_History int, @PartID_History int
						DECLARE @UnitState NVARCHAR(200)
						SELECT @UnitState=[Description] FROM mesUnitState WHERE ID=@UnitStateID

						IF NOT EXISTS (SELECT TOP 1 * FROM mesHistory WHERE UnitID= @unitId AND UnitStateID=@UnitStateID ) 
						BEGIN
							--SET @strOutput='ERROR:没有过站记录('+@UnitState+')  不允许回退没有扫描过的站。'
							--SET @strOutput='ERROR: can t found history of the '+ @UnitState
							SET @strOutput= '70061'
							SELECT @strOutput strOutput
							RETURN
						END 
						SELECT TOP 1  @StationId_History=StationID,@PODID_History=ProductionOrderID,@PartID_History=PartID 
						FROM mesHistory WHERE UnitID= @unitId AND UnitStateID= @UnitStateID ORDER BY ID

						select @LineId_History=LineID from mesStation where id=@StationId_History
						update u set u.UnitStateID=@UnitStateID,u.StationID=@StationId_History,u.LineID=@LineId_History,u.EmployeeID=@EmployeeID,u.LastUpdate=GETDATE()
						from mesUnit u join mesUnitDetail ud on u.ID=ud.UnitID 
						join mesPackage p on p.id=ud.InmostPackageID
						where p.SerialNumber=@strSNFormat and Stage=1

			
						UPDATE A SET A.ShipmentDetailID =null,A.ShipmentTime=null,ShipmentInterID=null,ShipmentParentID=null,LastUpdate=getdate()
						FROM mesPackage A WHERE SerialNumber = @strSNFormat and Stage=1
						
						UPDATE CO_WH_ShipmentEntryNew SET FOutSN=ISNULL(FOutSN,0)-1 WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID --and FStatus in (0,1,2)
						-- 2022-05-05 增加  StatusID = 0
						UPDATE mesPackage SET CurrentCount=CurrentCount-1,StatusID = 0  WHERE ID=@CurrShipmentPalletID
			

						IF  EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID AND FOutSN <> FCTN and FOutSN<>0)
						BEGIN
							UPDATE CO_WH_ShipmentEntryNew SET FStatus=1 WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID
						end 
						IF  EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID AND  FOutSN=0)
						BEGIN
							UPDATE CO_WH_ShipmentEntryNew SET FStatus=0 WHERE FInterID=@CurrShipmentEnterID and FDetailID=@CurrShipmentDetailID
						end
			
						IF  EXISTS( SELECT 1 FROM CO_WH_ShipmentEntryNew A
								INNER JOIN CO_WH_ShipmentNew B ON A.FInterID=B.FInterID
								WHERE B.FBillNO=@strSNbuf AND A.FOutSN<>a.FCTN )
							BEGIN
								UPDATE CO_WH_ShipmentNew SET FStatus=1 WHERE FBillNO=@strSNbuf
							END

						IF not exists (select 1 from mespackage where ShipmentParentID=@CurrShipmentPalletID )
							BEGIN
							insert into mesPackageHistory
							select @CurrShipmentPalletID,10,@stationID,@EmployeeId,getdate()
							UPDATE mesPackage SET ShipmentInterID=null, ShipmentTime=null WHERE ID=@CurrShipmentPalletID
							END

						INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
						SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesUnitDetail A 
							INNER JOIN mesUnit B ON A.UnitID=B.ID
							inner join mesPackage P on P.ID=A.InmostPackageID
						WHERE P.SerialNumber=@strSNFormat and Stage=1

						insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,[Time])
						select id,9,@stationID,@EmployeeId,getdate() from mesPackage where SerialNumber=@strSNFormat and Stage=1
						SET @strOutput=1
					COMMIT TRANSACTION Task
					SELECT @strOutput strOutput
				END TRY
				BEGIN CATCH
					IF @@TRANCOUNT >0 
					BEGIN
						ROLLBACK TRANSACTION
					END
					SELECT ERROR_MESSAGE() strOutput
				END CATCH
			END
			";
        return await DapperConn.QueryFirstAsync<SqlOutputStr>(S_Sql, null, null, I_DBTimeout, null);
    }
    public async Task<SqlOutputStr> uspUnpackShipmentPalletAsync(string BillNO, string shippingPallet, string xmlProdOrder, string xmlPartID, string xmlStation, string xmlExtraData)
    {
        string S_Sql = string.Empty;
        S_Sql = $@"						
			BEGIN
				DECLARE @strSNFormat      nvarchar(200) = '{shippingPallet}',			--ShipmentPalletSN		
						@xmlProdOrder     nvarchar(2000) = '{xmlProdOrder}', 
						@xmlPart          nvarchar(2000) = '{xmlPartID}',  
						@xmlStation       nvarchar(2000) = '{xmlStation}', 
						@xmlExtraData     nvarchar(2000) = '{xmlExtraData}',
						@strSNbuf         nvarchar(200) = '{BillNO}',			--BillNo
						@strOutput        nvarchar(200) 		--0：全部完成 1：正常执行 其他：错误代码 

				declare	@prodID				int,
						@partID				int,
						@stationID			int,
						@routeID			int,
						@lineID				int,
						@stationTypeID		int,
						@UnitStateID		int,
						@EmployeeId			int,
						@PackageID			int,
						@stage				int,
						@idoc				int

				--读取xmlPart参数值
		

					exec sp_xml_preparedocument @idoc output, @xmlStation
					SELECT @stationID=StationId
					FROM   OPENXML (@idoc, '/Station',2)
							WITH (StationId		  int			'@StationId')   
					exec sp_xml_removedocument @idoc
					IF isnull(@stationID,'') = ''
					BEGIN
						SET @strOutput= '20077'
						SELECT @strOutput strOutput
						RETURN
					END

					exec sp_xml_preparedocument @idoc output, @xmlExtraData
					SELECT @EmployeeId=EmployeeId
					FROM   OPENXML (@idoc, '/ExtraData',2)
							WITH (EmployeeId		  int		'@EmployeeId')   
					exec sp_xml_removedocument @idoc
					IF isnull(@stationID,'') = ''
					BEGIN
						SET @strOutput= '20077'
						SELECT @strOutput strOutput
						RETURN
					END
	
				-- select  *from syslanguage where description like '%state%'
				-- select  * from sysLanguage where code='20176'--20204
				BEGIN TRY
					DECLARE @CurrProductionOrderID	int,
							@CurrShipmentDetailID		int,
							--@ShipmentDetailID2	int,
							@CurrOutSN int,
							@CurrShipmentPalletID int,
							@CurrShipmentEnterID int,
							@CurrShipmentEnterID2 int
				

					SET @strOutput = 1
		 
					-- not exists BoxSN
					IF not EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat and stage=3)
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- BoxSN no shipmentPallet relationship
					IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat and Stage=3  AND ISNULL(ShipmentInterID,0)=0)
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- shipPallet status is wrong
					IF not EXISTS(SELECT 1 FROM CO_WH_ShipmentNew WHERE FBillNO = @strSNbuf and FStatus  in (1,2,3,4) )
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- not correct BillNo & BoxSN relationship
					select @CurrShipmentEnterID2=isnull(s.FInterID,0) from CO_WH_ShipmentNew s 
					join CO_WH_ShipmentEntryNew se on s.FInterID=se.FInterID where s.FBillNO=@strSNbuf
		
					select @CurrShipmentEnterID=isnull(ShipmentInterID,0),@CurrShipmentPalletID=isnull(ID,0)
					from mesPackage  where SerialNumber=@strSNFormat and Stage=3

					if (@CurrShipmentEnterID<>@CurrShipmentEnterID2) or (@CurrShipmentEnterID=0) 
					begin
					SET @strOutput='20183'
						SELECT @strOutput strOutput
						RETURN
					end

					 if exists(select 1 from CO_WH_ShipmentEntryNew where FInterID=@CurrShipmentEnterID and isnull(FOutSN,0)<=0)
					 begin
					 SET @strOutput='2000311'
						SELECT @strOutput strOutput
						RETURN
					 end
		
					declare @PackageDetailDefID_ShipmentDetailID int,@PackageDetailDefID_ShipmentEnterID int,@PackageDetailDefID_ShipmentParentID int
					select  @PackageDetailDefID_ShipmentDetailID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentDetailID'
					select  @PackageDetailDefID_ShipmentEnterID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentInterID'
					select  @PackageDetailDefID_ShipmentParentID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentParentID'
					if ( @PackageDetailDefID_ShipmentDetailID=0 or @PackageDetailDefID_ShipmentEnterID=0 or @PackageDetailDefID_ShipmentEnterID=0 )
					begin
					set @strOutput='20200'
					--select  *from sysLanguage where Description like '%parameter%'
					return
					end

					------base on Sn to find Route and unitstateID for update traget
					declare @CurrStateID int,@ProductionOrderID int,@unitId int,@StationID_Pre int
					CREATE TABLE #TempRoute
					(	
						ID				int,
						UnitStateID		int
					)
					select  top 1 @unitId=ud.UnitID,@CurrStateID=UnitStateID,@PartID=u.PartID,@LineID=u.LineID,@ProductionOrderID=u.ProductionOrderID,@StationID_Pre=u.StationID 
					from mesUnitDetail ud
					join mesUnit u on ud.UnitID=u.ID
					join mesPackage p on ud.InmostPackageID=p.id where  p.ShipmentInterID=@CurrShipmentEnterID and p.Stage=1
			
						--SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
					DECLARE	@PartFamilyID INT
					select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID

					select top 1 @routeID = RouteID
					from mesRouteMap
					where
						(LineID = @LineID or LineID is NULL OR LineID=0) AND
						(PartID = @PartID or PartID is NULL OR PartID=0) AND
						(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
						(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
					order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc

						IF ISNULL(@routeID,'')=''
						BEGIN
							SET @strOutput='20195'
							SELECT @strOutput strOutput
							RETURN
						END
			
						--根据route类型获取上一站UnitstateID
						IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
						BEGIN
							IF(SELECT COUNT(1) FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
								WHERE RouteID=@routeID)>1
							BEGIN
								SET @strOutput='20203'
								SELECT @strOutput strOutput
								RETURN
							END

							SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
							WHERE RouteID=@routeID
						END
						ELSE
						BEGIN
							INSERT INTO #TempRoute(ID,UnitStateID)
							SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
							FROM mesRouteDetail where RouteID=@routeID

							SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
								(SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
						END

						IF ISNULL(@UnitStateID,'')=''
						BEGIN
							SET @strOutput='20131'
								SELECT @strOutput strOutput
								RETURN
							END

					------end unitstateID for update unit traget

					--declare @AfterMovedShippingPalletUnitStateID varchar(100)
					--select @AfterMovedShippingPalletUnitStateID=isnull(value,'') from mesStationConfigSetting where StationID=@stationID and name='AfterMovedShippingPalletUnitStateID'
					--if @AfterMovedShippingPalletUnitStateID='' 
					--begin
					--set @strOutput='20199'
					----select  *from sysLanguage where Description like '%parameter%'
					--return
					--end

					BEGIN TRANSACTION Task

						-- record for shipPallet
						--insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						--select id,@PackageDetailDefID_ShipmentDetailID,ShipmentDetailID from mesPackage where SerialNumber=@strSNFormat and stage=3
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentEnterID,ShipmentInterID from mesPackage where SerialNumber=@strSNFormat and stage=3
						--insert into mesPackageDetail
						--select id,3,ShipmentParentID from mesPackage where SerialNumber=@strSNFormat and stage=3
						-- record for Box
			
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentDetailID,ShipmentDetailID from mesPackage where ShipmentInterID=@CurrShipmentEnterID and Stage=1
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentEnterID,ShipmentInterID from mesPackage where ShipmentInterID=@CurrShipmentEnterID and Stage=1
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentParentID,ShipmentParentID from mesPackage where ShipmentInterID=@CurrShipmentEnterID and Stage=1
						-- package history for pallet
						insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,[Time])
						select @CurrShipmentPalletID,10,@stationID,@EmployeeId,getdate()
						-- package for history for Box
						insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,[Time])
						select id,9,@stationID,@EmployeeId,getdate() from mesPackage where ShipmentInterID=@CurrShipmentEnterID and Stage=1
			

						-- update FG unitState
						declare @StationId_History    int, @LineId_History int,@PODID_History int, @PartID_History int
						DECLARE @UnitState NVARCHAR(200)
						SELECT @UnitState=[Description] FROM mesUnitState WHERE ID=@UnitStateID
			

						IF NOT EXISTS (SELECT TOP 1 * FROM mesHistory WHERE UnitID= @unitId AND UnitStateID=@UnitStateID ) 
						BEGIN
							--SET @strOutput='ERROR:没有过站记录('+@UnitState+')  不允许回退没有扫描过的站。'
							--SET @strOutput='ERROR: can t found history of the '+ @UnitState
							SET @strOutput= '70061'
							SELECT @strOutput strOutput
							RETURN
						END 
						SELECT TOP 1  @StationId_History=StationID,@PODID_History=ProductionOrderID,@PartID_History=PartID 
						FROM mesHistory WHERE UnitID= @unitId AND UnitStateID= @UnitStateID ORDER BY ID

						select @LineId_History=LineID from mesStation where id=@StationId_History
						INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
						SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesUnitDetail A 
							INNER JOIN mesUnit B ON A.UnitID=B.ID
							inner join mesPackage P on P.ID=A.InmostPackageID
						WHERE p.ShipmentInterID=@CurrShipmentEnterID and p.Stage=1

						update u set u.UnitStateID=@UnitStateID,u.StationID=@StationId_History,u.LineID=@LineId_History,u.EmployeeID=@EmployeeID,u.LastUpdate=GETDATE()
						from mesUnit u join mesUnitDetail ud on u.ID=ud.UnitID 
						join mesPackage p on p.id=ud.InmostPackageID
						where p.ShipmentInterID=@CurrShipmentEnterID and p.Stage=1

						-- update package for shipmentdata
						UPDATE A SET A.ShipmentDetailID =null,A.ShipmentTime=null,ShipmentInterID=null,ShipmentParentID=null,LastUpdate=getdate()
						FROM mesPackage A WHERE ShipmentInterID=@CurrShipmentEnterID and Stage in (1,3)
			
						UPDATE mesPackage SET CurrentCount=0,StatusID = 0  WHERE ID=@CurrShipmentPalletID
			
			
						UPDATE CO_WH_ShipmentEntryNew SET FOutSN=0,FStatus=0 WHERE FInterID=@CurrShipmentEnterID 
						UPDATE CO_WH_ShipmentNew SET FStatus=1 WHERE FBillNO=@strSNbuf

						SET @strOutput=1
			
					COMMIT TRANSACTION Task
					SELECT @strOutput strOutput
				END TRY
				BEGIN CATCH
				SET @strOutput = -1
					IF @@TRANCOUNT >0 
					BEGIN
						ROLLBACK TRANSACTION
					END
					SELECT ERROR_MESSAGE() strOutput
				END CATCH
			END
			";
        return await DapperConn.QueryFirstAsync<SqlOutputStr>(S_Sql, null, null, I_DBTimeout, null);
    }
    public async Task<SqlOutputStr> uspUnpackShipmentPalletAsync_P(string BillNO, string shippingPallet, string xmlProdOrder, string xmlPartID, string xmlStation, string xmlExtraData)
    {
        string S_Sql = string.Empty;
        S_Sql = $@"						
			BEGIN
				DECLARE @strSNFormat      nvarchar(200) = '{shippingPallet}',			--ShipmentPalletSN		
						@xmlProdOrder     nvarchar(2000) = '{xmlProdOrder}', 
						@xmlPart          nvarchar(2000) = '{xmlPartID}',  
						@xmlStation       nvarchar(2000) = '{xmlStation}', 
						@xmlExtraData     nvarchar(2000) = '{xmlExtraData}',
						@strSNbuf         nvarchar(200) = '{BillNO}',			--BillNo
						@strOutput        nvarchar(200) 		--0：全部完成 1：正常执行 其他：错误代码 

				declare	@prodID				int,
						@partID				int,
						@stationID			int,
						@routeID			int,
						@lineID				int,
						@stationTypeID		int,
						@UnitStateID		int,
						@EmployeeId			int,
						@PackageID			int,
						@stage				int,
						@idoc				int

				--读取xmlPart参数值
		

					exec sp_xml_preparedocument @idoc output, @xmlStation
					SELECT @stationID=StationId
					FROM   OPENXML (@idoc, '/Station',2)
							WITH (StationId		  int			'@StationId')   
					exec sp_xml_removedocument @idoc
					IF isnull(@stationID,'') = ''
					BEGIN
						SET @strOutput= '20077'
						SELECT @strOutput strOutput
						RETURN
					END

					exec sp_xml_preparedocument @idoc output, @xmlExtraData
					SELECT @EmployeeId=EmployeeId
					FROM   OPENXML (@idoc, '/ExtraData',2)
							WITH (EmployeeId		  int		'@EmployeeId')   
					exec sp_xml_removedocument @idoc
					IF isnull(@stationID,'') = ''
					BEGIN
						SET @strOutput= '20077'
						SELECT @strOutput strOutput
						RETURN
					END
	
				-- select  *from syslanguage where description like '%state%'
				-- select  * from sysLanguage where code='20176'--20204
				BEGIN TRY
					DECLARE @CurrProductionOrderID	int,
							@CurrShipmentDetailID		int,
							--@ShipmentDetailID2	int,
							@CurrOutSN int,
							@CurrShipmentPalletID int,
							@CurrShipmentEnterID int,
							@CurrShipmentEnterID2 int
				

					SET @strOutput = 1
		 
					-- not exists BoxSN
					IF not EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat and stage=3)
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- BoxSN no shipmentPallet relationship
					IF EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber = @strSNFormat and Stage=3  AND ISNULL(ShipmentInterID,0)=0)
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- shipPallet status is wrong
					IF not EXISTS(SELECT 1 FROM CO_WH_ShipmentNew WHERE FBillNO = @strSNbuf and FStatus  in (1,2,3,4) )
					BEGIN
						SET @strOutput='20003'
						SELECT @strOutput strOutput
						RETURN
					END
					-- not correct BillNo & BoxSN relationship
					select @CurrShipmentEnterID2=isnull(s.FInterID,0) from CO_WH_ShipmentNew s 
					join CO_WH_ShipmentEntryNew se on s.FInterID=se.FInterID where s.FBillNO=@strSNbuf
		
					select @CurrShipmentEnterID=isnull(ShipmentInterID,0),@CurrShipmentPalletID=isnull(ID,0)
					from mesPackage  where SerialNumber=@strSNFormat and Stage=3

					if (@CurrShipmentEnterID<>@CurrShipmentEnterID2) or (@CurrShipmentEnterID=0) 
					begin
					SET @strOutput='20183'
						SELECT @strOutput strOutput
						RETURN
					end

					 if exists(select 1 from CO_WH_ShipmentEntryNew where FInterID=@CurrShipmentEnterID and isnull(FOutSN,0)<=0)
					 begin
					 SET @strOutput='2000311'
						SELECT @strOutput strOutput
						RETURN
					 end
		
					declare @PackageDetailDefID_ShipmentDetailID int,@PackageDetailDefID_ShipmentEnterID int,@PackageDetailDefID_ShipmentParentID int
					select  @PackageDetailDefID_ShipmentDetailID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentDetailID'
					select  @PackageDetailDefID_ShipmentEnterID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentInterID'
					select  @PackageDetailDefID_ShipmentParentID=(isnull(id,0)) from luPackageDetailDef where Description='MovedShipmentParentID'
					if ( @PackageDetailDefID_ShipmentDetailID=0 or @PackageDetailDefID_ShipmentEnterID=0 or @PackageDetailDefID_ShipmentEnterID=0 )
					begin
					set @strOutput='20200'
					--select  *from sysLanguage where Description like '%parameter%'
					return
					end

					------base on Sn to find Route and unitstateID for update traget
					declare @CurrStateID int,@ProductionOrderID int,@unitId int,@StationID_Pre int
					CREATE TABLE #TempRoute
					(	
						ID				int,
						UnitStateID		int
					)
					select  top 1 @unitId=ud.UnitID,@CurrStateID=UnitStateID,@PartID=u.PartID,@LineID=u.LineID,@ProductionOrderID=u.ProductionOrderID,@StationID_Pre=u.StationID 
					from mesUnitDetail ud
					join mesUnit u on ud.UnitID=u.ID
					join mesPackage p on ud.InmostPackageID=p.id where  p.ShipmentInterID=@CurrShipmentEnterID and p.Stage=1
			
						SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
						IF ISNULL(@routeID,'')=''
						BEGIN
							SET @strOutput='20195'
							SELECT @strOutput strOutput
							RETURN
						END
			
						--根据route类型获取上一站UnitstateID
						IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
						BEGIN
							IF(SELECT COUNT(1) FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
								WHERE RouteID=@routeID)>1
							BEGIN
								SET @strOutput='20203'
								SELECT @strOutput strOutput
								RETURN
							END

							SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
								INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
							WHERE RouteID=@routeID
						END
						ELSE
						BEGIN
							INSERT INTO #TempRoute(ID,UnitStateID)
							SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
							FROM mesRouteDetail where RouteID=@routeID

							SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
								(SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
						END

						IF ISNULL(@UnitStateID,'')=''
						BEGIN
							SET @strOutput='20131'
								SELECT @strOutput strOutput
								RETURN
							END

					------end unitstateID for update unit traget

					--declare @AfterMovedShippingPalletUnitStateID varchar(100)
					--select @AfterMovedShippingPalletUnitStateID=isnull(value,'') from mesStationConfigSetting where StationID=@stationID and name='AfterMovedShippingPalletUnitStateID'
					--if @AfterMovedShippingPalletUnitStateID='' 
					--begin
					--set @strOutput='20199'
					----select  *from sysLanguage where Description like '%parameter%'
					--return
					--end

					BEGIN TRANSACTION Task

						-- record for shipPallet
						--insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						--select id,@PackageDetailDefID_ShipmentDetailID,ShipmentDetailID from mesPackage where SerialNumber=@strSNFormat and stage=3
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentEnterID,ShipmentInterID from mesPackage where SerialNumber=@strSNFormat and stage=3
						--insert into mesPackageDetail
						--select id,3,ShipmentParentID from mesPackage where SerialNumber=@strSNFormat and stage=3
						-- record for Box
			
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentDetailID,ShipmentDetailID from mesPackage where ShipmentInterID=@CurrShipmentEnterID and Stage=1
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentEnterID,ShipmentInterID from mesPackage where ShipmentInterID=@CurrShipmentEnterID and Stage=1
						insert into mesPackageDetail(PackageID,PackageDetailDefID,Content)
						select id,@PackageDetailDefID_ShipmentParentID,ShipmentParentID from mesPackage where ShipmentInterID=@CurrShipmentEnterID and Stage=1
						-- package history for pallet
						insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,[Time])
						select @CurrShipmentPalletID,10,@stationID,@EmployeeId,getdate()
						-- package for history for Box
						insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,[Time])
						select id,9,@stationID,@EmployeeId,getdate() from mesPackage where ShipmentInterID=@CurrShipmentEnterID and Stage=1
			

						-- update FG unitState
						declare @StationId_History    int, @LineId_History int,@PODID_History int, @PartID_History int
						DECLARE @UnitState NVARCHAR(200)
						SELECT @UnitState=[Description] FROM mesUnitState WHERE ID=@UnitStateID
			

						IF NOT EXISTS (SELECT TOP 1 * FROM mesHistory WHERE UnitID= @unitId AND UnitStateID=@UnitStateID ) 
						BEGIN
							--SET @strOutput='ERROR:没有过站记录('+@UnitState+')  不允许回退没有扫描过的站。'
							--SET @strOutput='ERROR: can t found history of the '+ @UnitState
							SET @strOutput= '70061'
							SELECT @strOutput strOutput
							RETURN
						END 
						SELECT TOP 1  @StationId_History=StationID,@PODID_History=ProductionOrderID,@PartID_History=PartID 
						FROM mesHistory WHERE UnitID= @unitId AND UnitStateID= @UnitStateID ORDER BY ID

						select @LineId_History=LineID from mesStation where id=@StationId_History
						INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
						SELECT A.UnitID,-100,@employeeId,@StationID,GETDATE(),GETDATE(),B.ProductionOrderID,B.PartID,1,1 FROM mesUnitDetail A 
							INNER JOIN mesUnit B ON A.UnitID=B.ID
							inner join mesPackage P on P.ID=A.InmostPackageID
						WHERE p.ShipmentInterID=@CurrShipmentEnterID and p.Stage=1

						update u set u.UnitStateID=@UnitStateID,u.StationID=@StationId_History,u.LineID=@LineId_History,u.EmployeeID=@EmployeeID,u.LastUpdate=GETDATE()
						from mesUnit u join mesUnitDetail ud on u.ID=ud.UnitID 
						join mesPackage p on p.id=ud.InmostPackageID
						where p.ShipmentInterID=@CurrShipmentEnterID and p.Stage=1

						-- update package for shipmentdata
						UPDATE A SET A.ShipmentDetailID =null,A.ShipmentTime=null,ShipmentInterID=null,ShipmentParentID=null,LastUpdate=getdate()
						FROM mesPackage A WHERE ShipmentInterID=@CurrShipmentEnterID and Stage in (1,3)
			
						UPDATE mesPackage SET CurrentCount=0,StatusID = 0  WHERE ID=@CurrShipmentPalletID
			
			
						UPDATE CO_WH_ShipmentEntryNew SET FOutSN=0,FStatus=0 WHERE FInterID=@CurrShipmentEnterID 
						UPDATE CO_WH_ShipmentNew SET FStatus=1 WHERE FBillNO=@strSNbuf

						SET @strOutput=1
			
					COMMIT TRANSACTION Task
					SELECT @strOutput strOutput
				END TRY
				BEGIN CATCH
				SET @strOutput = -1
					IF @@TRANCOUNT >0 
					BEGIN
						ROLLBACK TRANSACTION
					END
					SELECT ERROR_MESSAGE() strOutput
				END CATCH
			END
			";
        return await DapperConn.QueryFirstAsync<SqlOutputStr>(S_Sql, null, null, I_DBTimeout, null);
    }

	public async Task<OOBACheckParam> uspPackageCheckOOBAAsync(string boxSN, int stationId)
	{
        OOBACheckParam sqlOutputEntity = new OOBACheckParam();
        string S_Sql = string.Empty;
		S_Sql = $@"BEGIN
					declare	@StatusID				int,
							@BoxID					int,
							@ParentID				INT,
							@strOutput				VARCHAR(128)

		
					SET @strOutput='1'

					BEGIN TRY
						IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat)
						BEGIN
							SELECT @strOutput  ='20012'
							SELECT @strOutput strOutput 
							RETURN
						END

						SELECT @BoxID=ID,@StatusID=StatusID,@ParentID=ParentID 
							FROM mesPackage WHERE SerialNumber=@strSNFormat
						IF NOT EXISTS(SELECT 1 FROM mesUnitDetail WHERE InmostPackageID=@BoxID)
						BEGIN
							SET @strOutput='20129'
							SELECT @strOutput strOutput 
							RETURN
						END

						IF ISNULL(@ParentID,'')='' OR ISNULL(@StatusID,0)=0
						BEGIN
							SET @strOutput='20045'
							SELECT @strOutput strOutput 
							RETURN
						END

						--max 0816,已入库的不能做OOBA,Pallet才可以。
						DECLARE @stationId INT = {stationId}, @CheckOOBAUnitStatusID INT
					    IF NOT EXISTS(SELECT * FROM dbo.mesStationConfigSetting WHERE StationID = @stationId AND Name = 'CheckOOBAUnitStatusID')
						BEGIN
								SET @strOutput='please setup station attributes [""CheckOOBAUnitStatusID]'
								SELECT @strOutput strOutput 
								RETURN
						END
						SELECT @CheckOOBAUnitStatusID = CAST(Value AS INT) FROM dbo.mesStationConfigSetting WHERE StationID = @stationId AND Name = 'CheckOOBAUnitStatusID'
						if exists(SELECT * FROM mesUnitDetail a inner join mesUnit b on a.UnitID=b.ID 
								  WHERE a.InmostPackageID=@BoxID and b.UnitStateID<> @CheckOOBAUnitStatusID)
						begin
		   					SET @strOutput='20045'
							SELECT @strOutput strOutput 
							RETURN
						END

						--显示数据
						SELECT A.ProductionOrderID,A.Content,B.Description 
						INTO #tempProDetail FROM mesProductionOrderDetail A
						INNER JOIN luProductionOrderDetailDef B ON A.ProductionOrderDetailDefID=B.ID

						SELECT A.ID,P.SerialNumber AS PalletSN,A.CurrentCount,B.Content AS BoxQty,C.Content AS MPN,
							D.Content AS UPC,E.Content AS SCC,A.CurrPartID,A.CurrProductionOrderID
						 FROM mesPackage A 
						LEFT JOIN #tempProDetail B ON A.CurrProductionOrderID=B.ProductionOrderID AND B.Description='BoxQty'
						LEFT JOIN #tempProDetail C ON A.CurrProductionOrderID=C.ProductionOrderID AND C.Description='MPN'
						LEFT JOIN #tempProDetail D ON A.CurrProductionOrderID=D.ProductionOrderID AND D.Description='UPC'
						LEFT JOIN #tempProDetail E ON A.CurrProductionOrderID=E.ProductionOrderID AND E.Description='SCC'
						INNER JOIN mesPackage P ON A.ParentID=P.ID
						WHERE A.ID=@BoxID

						--打印数据
						SELECT B.Value AS SN FROM mesUnitDetail A 
							INNER JOIN mesSerialNumber B ON A.UnitID = B.UnitID
						WHERE A.InmostPackageID=@BoxID
						SELECT @strOutput strOutput 

						DROP TABLE #tempProDetail
					END TRY
					BEGIN CATCH
						SET @strOutput = ERROR_MESSAGE()
						SELECT @strOutput strOutput 
						RETURN
					END CATCH

				END";

		var outputDataset = await SqlSugarHelper.Db.Ado.GetDataSetAllAsync(S_Sql, new { strSNFormat = boxSN } );

		var resDataTable = outputDataset.Tables[outputDataset.Tables.Count - 1];

        sqlOutputEntity.strOutput = resDataTable?.Rows?[0]["strOutput"]?.ToString();
        if (sqlOutputEntity.strOutput == "1")
		{
			OOBACheckInfo tmpCi = new OOBACheckInfo();

            OOBACheckInfo ci = outputDataset.Tables[0].DataTableToModel<OOBACheckInfo>(tmpCi);

			sqlOutputEntity.BoxInformation = ci;

            List<string> SNList = new List<string>();
			foreach (DataRow item in outputDataset.Tables[1].Rows)
			{
				SNList.Add(item["SN"].ToString());
			}
			sqlOutputEntity.PrintSnList = SNList;
        }

		//try
		//{
  //          {
  //              SugarParameter[] pars = SqlSugarHelper.Db.Ado.GetParameters(new
  //              {
  //                  @strOutput = "",
  //                  @strSNFormat = boxSN,
  //                  @xmlProdOrder = "",
  //                  @xmlPart = "",
  //                  @xmlStation = "",
  //                  @xmlExtraData = "",
  //                  @strSNbuf = "",
  //              });
		//		pars[0].Direction = ParameterDirection.Output;


		//		var vvv = await SqlSugarHelper.Db.Ado.UseStoredProcedure().GetDataTableAsync("uspPackageCheckOOBA", pars);
  //              await Console.Out.WriteLineAsync(pars[0].Value.ToString());
  //              await Console.Out.WriteLineAsync(vvv.DataSet.Tables.Count.ToString();
  //          }
  //      }
		//catch (Exception ex)
		//{

		//	throw;
		//}



        return sqlOutputEntity;
	}

    public async Task<OOBACheckParam> uspPackageReworkOOBAAsync(string BoxSN, int StationID, int EmployeeId)
    {
        OOBACheckParam sqlOutputEntity = new OOBACheckParam();
        string S_Sql = string.Empty;
        S_Sql = @"
BEGIN
			DECLARE @StatusID INT, @BoxID INT, @ParentID INT,    @UnitStateID VARCHAR(64),@UnitStateIDs VARCHAR(200),	@strOutput NVARCHAR(200)
			SET @strOutput='1'
	 
			BEGIN TRY	

				---------------------------------------
				--增加通过站点获取需要设置跳转到哪个工序  20220824
				IF EXISTS (SELECT 1
						   FROM dbo.mesStationConfigSetting
						   WHERE Name='OOBANewUnitStateID' AND StationID=@StationID)BEGIN
					SELECT @UnitStateIDs=CAST(Value AS VARCHAR(200))
					FROM dbo.mesStationConfigSetting
					WHERE Name='OOBANewUnitStateID' AND StationID=@StationID
				END
				ELSE
				BEGIN
					SET @strOutput='ERROR: please setup station attribute < OOBANewUnitStateID >.'
					SELECT @strOutput strOutput
					RETURN
				END

				CREATE TABLE #tmpUnitStateIDs(
					id int,
					value nvarchar(max)
				)
				IF ISNULL(@UnitStateIDs,'') <> ''
				BEGIN
					INSERT INTO #tmpUnitStateIDs(id, value)
					SELECT * FROM dbo.F_Split(@UnitStateIDs,',')
				END
				ELSE
				BEGIN
					INSERT INTO #tmpUnitStateIDs(id, value)
					VALUES(1, -- id - int
					@UnitStateID -- value - nvarchar(max)
						)
				END
		
				DECLARE @UnitStateCount INT
				SELECT @UnitStateCount = COUNT(1) FROM #tmpUnitStateIDs
				---------------------------------------  

				SELECT TOP 1 @BoxID=ID, @StatusID=StatusID, @ParentID=ParentID
				FROM dbo.mesPackage
				WHERE SerialNumber=@strSNFormat AND Stage=1
				IF ISNULL(@BoxID, '')='' BEGIN
					SET @strOutput='20119'					
					SELECT @strOutput strOutput
					RETURN
				END
				IF @StatusID=0 OR ISNULL(@ParentID, '')='' 
				BEGIN
					SET @strOutput='20003'
					SELECT @strOutput strOutput
					RETURN
				END

				DECLARE @boxCount INT,@verifyCount INT

				SELECT @boxCount = COUNT(UnitID)
				FROM dbo.mesUnitDetail 
				WHERE InmostPackageID = @BoxID


				SELECT @verifyCount  = ISNULL(SUM(a.t),0)
				FROM (
				SELECT COUNT(b.UnitID) t
				FROM dbo.mesUnitDetail a
				JOIN dbo.mesHistory b ON b.UnitID = a.UnitID 
				JOIN #tmpUnitStateIDs c ON CAST(c.value AS INT) = b.UnitStateID
				WHERE a.InmostPackageID = @BoxID
				GROUP BY b.UnitID,b.UnitStateID
				) a
				IF @boxCount <> @verifyCount
				BEGIN
					SET @strOutput = '20045'
					SELECT @strOutput strOutput
					RETURN
				END
				-------------------------------------------------------20221013
				--针对OOBA站拆箱后，保留UPC条码，且需要设置站点为装箱前状态。
				DECLARE @IfKeepUPCSN VARCHAR(10) ='0'
				IF EXISTS (SELECT 1
						   FROM dbo.mesStationConfigSetting
						   WHERE Name='IsKeepUPCSN' AND StationID=@StationID)BEGIN
					SELECT @IfKeepUPCSN=CAST(Value AS VARCHAR(64))
					FROM dbo.mesStationConfigSetting
					WHERE Name='IsKeepUPCSN' AND StationID=@StationID
				END
				IF  @IfKeepUPCSN = '1'
				BEGIN
					BEGIN TRANSACTION TASK
					BEGIN try
						--修改PalletSN状态    
						UPDATE dbo.mesPackage
						SET CurrentCount=CASE WHEN ISNULL(CurrentCount, 0)=0 THEN NULL ELSE CurrentCount-1 END, StatusID=0
						WHERE ID=@ParentID
						UPDATE dbo.mesPackage SET ParentID=NULL WHERE ID=@BoxID

						--修改MultipackSN状态    
						UPDATE dbo.mesPackage SET CurrentCount=0, StatusID=0 WHERE ID=@BoxID
						INSERT INTO dbo.mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, [Time])
						VALUES(@BoxID, 4, @StationID, @employeeId, GETDATE())

						--修改FG条码状态    
						UPDATE A
						SET A.UnitStateID= c.UnitStateID, A.StationID=C.StationID, A.EmployeeID=@employeeId, A.LastUpdate=GETDATE(), A.ProductionOrderID=C.ProductionOrderID
						FROM dbo.mesUnit A
						INNER JOIN dbo.mesUnitDetail B ON A.ID=B.UnitID
						INNER JOIN dbo.mesHistory C ON A.ID=C.UnitID 
						JOIN #tmpUnitStateIDs d ON CAST(d.value AS INT) = c.UnitStateID
						WHERE B.InmostPackageID=@BoxID

						--FG插入历史记录
						INSERT INTO dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
						SELECT A.UnitID, -100, @employeeId, @StationID, GETDATE(), GETDATE(), B.ProductionOrderID, B.PartID, 1, 1
						FROM dbo.mesUnitDetail A
						INNER JOIN dbo.mesUnit B ON A.UnitID=B.ID
						WHERE A.InmostPackageID=@BoxID

						--修改UPC条码状态
						UPDATE A
						SET A.UnitStateID=c.UnitStateID, A.StationID=C.StationID, A.EmployeeID=@employeeId, A.LastUpdate=GETDATE(), A.ProductionOrderID=C.ProductionOrderID
						FROM dbo.mesUnit A
						INNER JOIN dbo.mesSerialNumber D ON d.UnitID = a.ID
						INNER JOIN dbo.mesUnitDetail B ON b.KitSerialNumber = d.Value
						INNER JOIN dbo.mesHistory C ON A.ID=C.UnitID 
						JOIN #tmpUnitStateIDs e ON CAST(e.value AS INT) = c.UnitStateID
						WHERE B.InmostPackageID=@BoxID          

						--插入UPC历史记录
						INSERT INTO dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
						SELECT B.ID, -100, @employeeId, @StationID, GETDATE(), GETDATE(), B.ProductionOrderID, B.PartID, 1, 1
						FROM dbo.mesUnitDetail A
							 INNER JOIN dbo.mesSerialNumber S ON A.KitSerialNumber=S.Value
							 INNER JOIN dbo.mesUnit B ON S.UnitID=B.ID
						WHERE A.InmostPackageID=@BoxID
						--解除箱码
						UPDATE dbo.mesUnitDetail
						SET reserved_19=InmostPackageID,InmostPackageID = NULL
						WHERE InmostPackageID=@BoxID
						COMMIT TRANSACTION Task
						SELECT @strOutput strOutput
						RETURN
					END TRY
					BEGIN CATCH
						IF @@TRANCOUNT>0 BEGIN
							ROLLBACK TRANSACTION
						END
						SET @strOutput=ERROR_MESSAGE()
						SELECT @strOutput strOutput
						RETURN
					END CATCH
				END
				-------------------------------------------------------
				BEGIN TRANSACTION TASK
				DECLARE @IsCreateUPCSN VARCHAR(40)
				SELECT TOP 1 @IsCreateUPCSN=ISNULL(C.Content, 0)
				FROM dbo.mesUnit A
					 INNER JOIN dbo.mesUnitDetail B ON A.ID=B.UnitID
					 INNER JOIN dbo.mesProductionOrderDetail C ON A.ProductionOrderID=C.ProductionOrderID
					 INNER JOIN dbo.luProductionOrderDetailDef D ON C.ProductionOrderDetailDefID=D.ID AND D.Description='IsCreateUPCSN'
				WHERE B.InmostPackageID=@BoxID
				IF(ISNULL(@IsCreateUPCSN, '0')='0')BEGIN
					SET @IsCreateUPCSN='0'
				END

				--修改PalletSN状态    
				UPDATE dbo.mesPackage
				SET CurrentCount=CASE WHEN ISNULL(CurrentCount, 0)=0 THEN NULL ELSE CurrentCount-1 END, StatusID=0
				WHERE ID=@ParentID
				UPDATE dbo.mesPackage SET ParentID=NULL WHERE ID=@BoxID

				--修改MultipackSN状态    
				UPDATE dbo.mesPackage SET CurrentCount=0, StatusID=0 WHERE ID=@BoxID
				INSERT INTO dbo.mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, [Time])
				VALUES(@BoxID, 4, @StationID, @employeeId, GETDATE())

				--修改条码状态    
				UPDATE A
				SET A.UnitStateID=c.UnitStateID, A.StationID=C.StationID, A.EmployeeID=@employeeId, A.LastUpdate=GETDATE(), A.ProductionOrderID=C.ProductionOrderID
				FROM dbo.mesUnit A
					 INNER JOIN dbo.mesUnitDetail B ON A.ID=B.UnitID
					 INNER JOIN dbo.mesHistory C ON A.ID=C.UnitID 
					 JOIN #tmpUnitStateIDs d ON CAST(d.value AS int) = c.UnitStateID
				WHERE B.InmostPackageID=@BoxID

				INSERT INTO dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
				SELECT A.UnitID, -100, @employeeId, @StationID, GETDATE(), GETDATE(), B.ProductionOrderID, B.PartID, 1, 1
				FROM dbo.mesUnitDetail A
				INNER JOIN dbo.mesUnit B ON A.UnitID=B.ID
				WHERE A.InmostPackageID=@BoxID
				IF NOT EXISTS (SELECT 1
							   FROM dbo.mesUnitDetail A
									INNER JOIN dbo.mesSerialNumber B ON A.UnitID=B.UnitID
							   WHERE A.InmostPackageID=@BoxID AND B.Value=@strSNFormat)BEGIN
					INSERT INTO dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
					SELECT B.ID, -100, @employeeId, @StationID, GETDATE(), GETDATE(), B.ProductionOrderID, B.PartID, 1, 1
					FROM dbo.mesUnitDetail A
						 INNER JOIN dbo.mesSerialNumber S ON A.KitSerialNumber=S.Value
						 INNER JOIN dbo.mesUnit B ON S.UnitID=B.ID
					WHERE A.InmostPackageID=@BoxID
				END

				--max FGPrintUPC需要重新产生UPC  
				IF(@IsCreateUPCSN='1')BEGIN
					DECLARE @Time VARCHAR(40)
					SELECT @Time=REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR, GETDATE(), 120), '-', ''), ' ', ''), ':', '')
					UPDATE C
					SET C.Value=C.Value+'-'+@Time
					FROM dbo.mesUnit A
						 INNER JOIN dbo.mesSerialNumber C ON A.ID=C.UnitID
						 INNER JOIN dbo.mesUnitDetail B ON C.Value=B.KitSerialNumber
					WHERE B.InmostPackageID=@BoxID
					UPDATE A
					SET A.UnitStateID=-100, A.StationID=@StationID
					FROM dbo.mesUnit A
						 INNER JOIN dbo.mesSerialNumber C ON A.ID=C.UnitID
						 INNER JOIN dbo.mesUnitDetail B ON C.Value=B.KitSerialNumber
					WHERE B.InmostPackageID=@BoxID
				END
				ELSE BEGIN
					--判断UPC和FG是不是同一个条码    
					IF NOT EXISTS (SELECT 1
								   FROM dbo.mesUnitDetail A
										INNER JOIN dbo.mesSerialNumber B ON A.UnitID=B.UnitID
								   WHERE A.InmostPackageID=@BoxID AND B.Value=@strSNFormat)BEGIN
						UPDATE A
						SET A.UnitStateID=e.UnitStateID, A.StationID=@StationID, A.EmployeeID=@employeeId, A.LastUpdate=GETDATE()
						FROM dbo.mesUnit A
							 INNER JOIN dbo.mesSerialNumber C ON A.ID=C.UnitID
							 INNER JOIN dbo.mesUnitDetail B ON C.Value=B.KitSerialNumber
							 JOIN dbo.mesHistory e ON e.UnitID = a.ID
							 JOIN #tmpUnitStateIDs d ON CAST(d.value AS INT) = e.UnitStateID
						WHERE B.InmostPackageID=@BoxID
					END
				END
				UPDATE dbo.mesUnitDetail
				SET reserved_19=InmostPackageID
				WHERE InmostPackageID=@BoxID
				UPDATE dbo.mesUnitDetail
				SET InmostPackageID=NULL, KitSerialNumber=NULL
				WHERE InmostPackageID=@BoxID
				COMMIT TRANSACTION Task
				SELECT @strOutput strOutput
			END TRY
			BEGIN CATCH
				IF @@TRANCOUNT>0 BEGIN
					ROLLBACK TRANSACTION
				END
				SET @strOutput=ERROR_MESSAGE()
				SELECT @strOutput strOutput
				RETURN
			END CATCH
		END";

        var outputDataset = await SqlSugarHelper.Db.Ado.GetDataSetAllAsync(S_Sql, new { strSNFormat = BoxSN, StationID=StationID, employeeId =EmployeeId});

        var resDataTable = outputDataset.Tables[outputDataset.Tables.Count - 1];

        sqlOutputEntity.strOutput = resDataTable?.Rows?[0]["strOutput"]?.ToString();

        return sqlOutputEntity;
    }
    public async Task<mesUnitSerial> GetAndCheckUnitInfoAsync(string barcode, string POID, string PartID)
    {

        string sql = @"SELECT b.*,d.PartFamilyID,a.*
                        FROM dbo.mesSerialNumber a
                        JOIN dbo.mesUnit b ON b.ID = a.UnitID
                        JOIN dbo.mesProductionOrder c ON b.ProductionOrderID = c.ID AND b.PartID = c.PartID
                        JOIN dbo.mesPart d ON d.ID = b.PartID
                        WHERE a.Value = '" + barcode + "' AND b.PartID = " + PartID + " AND b.ProductionOrderID = " + POID + "";
        return await SqlSugarHelper.Db.SqlQueryable<mesUnitSerial>(sql).FirstAsync();
    }
    public List<mesProductStructure> GetmesProductStructure2(string ParentPartID, string StationTypeID)
    {

        string S_Sql = "select * from mesProductStructure where ParentPartID='" + ParentPartID +
            "' and StationTypeID='" + StationTypeID + "' and Status=1";
        return  SqlSugarHelper.Db.SqlQueryable<mesProductStructure>(S_Sql).ToList();
    }

    public async Task<SqlOutputStr> uspQcReleaseMachineAsync(string SN, string xmlPart, string xmlStation)
    {

		string S_Sql = $@"
						BEGIN
							BEGIN TRAN
							BEGIN TRY
								DECLARE
								@strSNFormat      nvarchar(200) = '{SN}',		-- 条码
								@xmlProdOrder     nvarchar(200) = '', 
								@xmlPart          nvarchar(200) = '{xmlPart}',  
								@xmlStation       nvarchar(200) = '{xmlStation}', 
								@xmlExtraData     nvarchar(200) = '',
								@strSNbuf         nvarchar(200) = '',			
								@strOutput        nvarchar(200) = '1'
								IF NOT EXISTS(SELECT 1 FROM mesSerialNumber WHERE Value=@strSNFormat)
								BEGIN
									SELECT @strOutput strOutput
									RETURN
								END

								SELECT F.reserved_01 INTO #TempRelease FROM mesSerialNumber A 
								INNER JOIN mesUnitComponent B ON A.UnitID=B.UnitID
								INNER JOIN mesPart C ON B.ChildPartID=C.ID
								INNER JOIN mesPartDetail D ON C.ID=D.PartID
								INNER JOIN luPartDetailDef E ON D.PartDetailDefID=E.ID
								INNER JOIN mesUnitDetail F ON B.ChildUnitID=F.UnitID
								WHERE E.Description='ScanType' AND D.Content='4' AND A.Value=@strSNFormat

								UPDATE M SET M.RuningCapacityQuantity=0,StatusID=1 FROM mesMachine M
								WHERE EXISTS(SELECT 1 FROM #TempRelease F WHERE M.SN=F.reserved_01)

								UPDATE S SET reserved_03 = '2' FROM mesUnitDetail S WHERE reserved_03 = '1' AND EXISTS
								(SELECT 1 FROM #TempRelease F WHERE S.reserved_01=F.reserved_01)

								drop table #TempRelease
								COMMIT TRAN
								SELECT @strOutput strOutput
							END TRY
							BEGIN CATCH
								ROLLBACK TRAN
								set @strOutput=ERROR_MESSAGE()
								SELECT @strOutput strOutput
							END CATCH
						END";
		return await DapperConn.QueryFirstOrDefaultAsync<SqlOutputStr>(S_Sql, null, null, I_DBTimeout, null);
    }
    public async Task<string> uspSetProductionOrderCountAsync( string PoId,string StationId)
    {
		string strSql = $@"
				DECLARE @stationID INT = {StationId},@prodId INT = {PoId},@unitID INT,
				  @stationTypeID INT,@lineID INT, @strOutput VARCHAR(200) = '1'

				BEGIN TRY	
				BEGIN TRAN
					IF exists(
					select 1 from mesStation s 
					join V_StationTypeInfo sti on s.StationTypeID=sti.StationTypeID and sti.DetailDef='StationTypeType' and sti.Content='TT'
						where s.ID=@stationID ) 
						BEGIN
							SELECT @strOutput strOutput
							RETURN
						end


					SELECT @lineID=LineID,@stationTypeID=StationTypeID FROM mesStation WHERE ID=@stationId
					--判断是否存在记录
					IF EXISTS(SELECT 1 FROM mesProductionOrderConsumeInfo WHERE ProductionOrderID=@prodId
						AND StationTypeID=@stationTypeID AND LineID=@lineID AND StationID=@stationId)
					BEGIN
						UPDATE mesProductionOrderConsumeInfo SET ConsumeQTY=ISNULL(ConsumeQTY,0)+1
						WHERE ProductionOrderID=@prodId AND StationTypeID=@stationTypeID AND LineID=@lineID AND StationID=@stationId
					END
					ELSE
					BEGIN
						INSERT INTO mesProductionOrderConsumeInfo(ProductionOrderID,LineID,StationTypeID,StationID,ConsumeQTY)
						VALUES (@prodId,@lineID,@stationTypeID,@stationID,1)
					END
					SELECT @strOutput strOutput
					COMMIT TRAN
				END TRY
				BEGIN CATCH
						IF @@TRANCOUNT >0 
						BEGIN
							ROLLBACK TRAN
						END
						SET @strOutput = ERROR_MESSAGE()
		
						SELECT @strOutput strOutput 
						RETURN
				END CATCH";

        var vQuery = await DapperConn.ExecuteScalarAsync(strSql, null, null, I_DBTimeout, null);

        return vQuery?.ToString();
    }
    public async Task<string> CheckHistoryWithSNAsync(string SN,  string StationId)
    {
        string strSql = $@"
				SELECT 1
				FROM dbo.mesHistory a 
				JOIN dbo.mesSerialNumber b ON b.UnitID = a.UnitID
				WHERE b.Value = '{SN}' AND a.StationID = {StationId}";

        var vQuery = await DapperConn.ExecuteScalarAsync(strSql, null, null, I_DBTimeout, null);

        return vQuery?.ToString();
    }
    public async Task<string> IsDiagramSFCLastStation(string stationTypeId, string routeId)
    {
        string strSql = $@"
				SELECT 1 FROM mesUnitoUTputState A
						LEFT JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID
						WHERE A.RouteID={routeId} and A.StationTypeID='{stationTypeId}' 
						AND ISNULL(B.Content,'')<>'TT' AND CurrStateID=0
						 AND A.OutputStateDefID=1";

        var vQuery = await DapperConn.ExecuteScalarAsync(strSql, null, null, I_DBTimeout, null);

        return vQuery?.ToString();
    }
    public async Task<string> IsTableFCLastStation(string stationTypeId, string routeId, string UnitStateID)
    {
        string strSql = $@"
				SELECT 1
                            FROM dbo.mesRouteDetail a
                            WHERE a.RouteID = {routeId} AND a.StationTypeID = {stationTypeId} AND a.UnitStateID = {UnitStateID} AND a.Sequence = (SELECT MAX(b.Sequence) FROM dbo.mesRouteDetail b WHERE b.RouteID = a.RouteID)";

        var vQuery = await DapperConn.ExecuteScalarAsync(strSql, null, null, I_DBTimeout, null);

        return vQuery?.ToString();
    }
    public async Task<string> IsExistBindChildPart(int mainUnitID, int childUnitID, string stationTypeId, string BatchNumber, string childPartId)
    {
        string strSql = $@"
				SELECT 1
            FROM dbo.mesUnitComponent a
            JOIN dbo.mesStation b ON b.ID = a.InsertedStationID
            WHERE b.StationTypeID = {stationTypeId} AND a.UnitID = {mainUnitID}  AND a.StatusID = 1  AND a.ChildPartID = {childPartId}";

        var vQuery = await DapperConn.ExecuteScalarAsync(strSql, null, null, I_DBTimeout, null);

        return vQuery?.ToString();
    }
}