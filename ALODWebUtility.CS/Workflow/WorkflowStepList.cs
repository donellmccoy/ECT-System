using System;
using System.Collections.Generic;
using System.Data;
using ALOD.Data;
using ALODWebUtility.DataAccess;

namespace ALODWebUtility.Workflow
{
    [Serializable]
    public class WorkflowStepList : IList<WorkflowStep>
    {
        protected List<WorkflowStep> _steps = new List<WorkflowStep>();
        protected byte _workflowId = 0;
        SqlDataStore _adapter;

        protected SqlDataStore Adapter
        {
            get
            {
                if (_adapter == null)
                {
                    _adapter = new SqlDataStore();
                }
                return _adapter;
            }
        }

        public WorkflowStepList GetSteps()
        {
            Adapter.ExecuteReader(StepReader, "core_workflow_sp_getStepsByWorkflow", _workflowId);
            return this;
        }

        public WorkflowStepList GetSteps(byte workflowId)
        {
            _workflowId = workflowId;
            return GetSteps();
        }

        public DataSet GetStepsAsDataSet()
        {
            GetSteps();
            return this.ToDataSet();
        }

        public DataSet GetStepsAsDataSet(byte workflowId)
        {
            _workflowId = workflowId;
            return GetStepsAsDataSet();
        }

        public WorkflowStepList GetStepsByStatus(byte workflowId, int status, bool isDeathCase)
        {
            _workflowId = workflowId;
            char deathStatus;
            if (isDeathCase)
            {
                deathStatus = 'Y';
            }
            else
            {
                deathStatus = 'N';
            }
            Adapter.ExecuteReader(StepReader, "core_workflow_sp_getStepsByWorkflowAndStatus", _workflowId, status, deathStatus);
            return this;
        }

        public DataSet GetStepsByStatusAsDataSet(byte workflowId, int status, bool isDeathCase)
        {
            GetStepsByStatus(workflowId, status, isDeathCase);
            return this.ToDataSet();
        }

        public DataSet ToDataSet()
        {
            DataSet data = new DataSet();
            bool topRow;
            int lastStatus = -1;

            DataSets.WorkflowStepsDataTable top = new DataSets.WorkflowStepsDataTable();
            top.TableName = "top";
            DataSets.WorkflowStepsDataTable children = new DataSets.WorkflowStepsDataTable();

            data.Tables.Add(top);
            data.Tables.Add(children);
            data.Relations.Add("steps", top.Columns["statusIn"], children.Columns["statusIn"], false);
            data.Relations[0].Nested = true;

            foreach (WorkflowStep item in _steps)
            {
                DataSets.WorkflowStepsRow row;
                topRow = (item.StatusIn != lastStatus);

                if (topRow)
                {
                    row = top.NewWorkflowStepsRow();
                    lastStatus = item.StatusIn;
                }
                else
                {
                    row = children.NewWorkflowStepsRow();
                }

                item.ToDataRow(row);

                if (topRow)
                {
                    top.Rows.Add(row);
                    children.ImportRow(row);
                }
                else
                {
                    children.Rows.Add(row);
                }
            }

            return data;
        }

        protected void StepReader(SqlDataStore adapter, IDataReader reader)
        {
            //0-stepId, 1-workflowId, 2-statusIn 3-statusOut, 4-displayText
            //5-stepType, 6-active, 7-displayOrder
            //8-statusInDescr, 9-statusOutDescr
            //10-groupInId, 11-groupInName, 12-groupOutId, 13-groupOutName
            //14-dbSignTemplate, 15-actionCount, 16-deathStatus, 17-memoTemplate

            WorkflowStep item = new WorkflowStep();

            item.Id = adapter.GetInt16(reader, 0);
            item.Workflow = adapter.GetByte(reader, 1);
            item.StatusIn = adapter.GetInt32(reader, 2);
            item.StatusOut = adapter.GetInt32(reader, 3);
            item.Text = adapter.GetString(reader, 4);

            item.Active = adapter.GetBoolean(reader, 6);
            item.DisplayOrder = adapter.GetByte(reader, 7);
            item.StatusInDescription = adapter.GetString(reader, 8);
            item.StatusOutDescription = adapter.GetString(reader, 9);

            item.GroupInId = adapter.GetByte(reader, 10);
            item.GroupInDescr = adapter.GetString(reader, 11);
            item.GroupOutId = adapter.GetByte(reader, 12);
            item.GroupOutDescr = adapter.GetString(reader, 13);
            item.DBSignTemplate = (ALOD.Core.Domain.DBSign.DBSignTemplateId)adapter.GetByte(reader, 14);
            item.ActionCount = adapter.GetInt32(reader, 15);
            item.DeathStatus = adapter.GetString(reader, 16, "A")[0];

            item.MemoTemplate = adapter.GetByte(reader, 17, 0);

            _steps.Add(item);
        }

        #region IList

        public int Count
        {
            get { return _steps.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public WorkflowStep this[int stepId]
        {
            get
            {
                foreach (WorkflowStep workstep in _steps)
                {
                    if (workstep.Id == stepId)
                    {
                        return workstep;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < _steps.Count; i++)
                {
                    if (_steps[i].Id == stepId)
                    {
                        _steps[i] = value;
                        break;
                    }
                }
            }
        }

        public void Add(WorkflowStep item)
        {
            _steps.Add(item);
        }

        public void Clear()
        {
            _steps.Clear();
        }

        public bool Contains(WorkflowStep item)
        {
            return _steps.Contains(item);
        }

        public void CopyTo(WorkflowStep[] array, int arrayIndex)
        {
            _steps.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WorkflowStep> GetEnumerator()
        {
            return _steps.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _steps.GetEnumerator();
        }

        public int IndexOf(WorkflowStep item)
        {
            return _steps.IndexOf(item);
        }

        public void Insert(int index, WorkflowStep item)
        {
            _steps.Insert(index, item);
        }

        public bool Remove(WorkflowStep item)
        {
            return _steps.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _steps.RemoveAt(index);
        }

        #endregion
    }
}
