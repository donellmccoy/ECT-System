using System;
using System.Linq;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_Tracking : System.Web.UI.Page
    {
        private ILineOfDutyDao _dao;
        private IDaoFactory _daoFactory;
        private LineOfDuty _lod;

        protected ILineOfDutyDao LODDao
        {
            get
            {
                if (_dao == null)
                {
                    _dao = DaoFactory.GetLineOfDutyDao();
                }

                return _dao;
            }
        }

        public ModuleType PageModuleType
        {
            get
            {
                return ModuleType.LOD;
            }
        }

        protected IDaoFactory DaoFactory
        {
            get
            {
                if (_daoFactory == null)
                {
                    _daoFactory = new NHibernateDaoFactory();
                }

                return _daoFactory;
            }
        }

        protected LineOfDuty LOD
        {
            get
            {
                if (_lod == null)
                {
                    _lod = LODDao.GetById(RequestId, false);
                }
                return _lod;
            }
        }

        protected int RequestId
        {
            get
            {
                return int.Parse(Request.QueryString["refId"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // TODO: CaseTracking is a UserControl that needs to be converted
                // CaseTracking.Initialize(this, (byte)PageModuleType, RequestId, LOD.CaseId, LOD.Status, GetUnitId());
            }
        }

        private int GetUnitId()
        {
            if (LOD.isAttachPas)
            {
                return LOD.MemberAttachedUnitId ?? 0;
            }
            else
            {
                return LOD.MemberUnitId;
            }
        }
    }
}
