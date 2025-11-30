using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="LineOfDutyAudit"/> entities.
    /// Handles audit operations for LOD cases including checking audit status, retrieving audit information, and saving different audit types (A1, JA, SG).
    /// </summary>
    public class LineOfDutyAuditDao : AbstractNHibernateDao<LineOfDutyAudit, int>, ILineOfDutyAuditDao
    {
        private SqlDataStore store = new SqlDataStore();

        /// <summary>
        /// Checks whether an audit has been initiated for a specific LOD case.
        /// </summary>
        /// <param name="lodid">The LOD ID.</param>
        /// <returns>A DataSet containing audit initiation status.</returns>
        public System.Data.DataSet CheckIfAuditInitiated(int lodid)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, System.Text.Encoding.ASCII);

            return store.ExecuteDataSet("Form348_Audit_CheckIfAuditInitiated", lodid);
        }

        /// <summary>
        /// Retrieves audit information for a specific LOD case.
        /// </summary>
        /// <param name="lodid">The LOD ID.</param>
        /// <returns>A DataSet containing audit information.</returns>
        public System.Data.DataSet GetAuditInfo(int lodid)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, System.Text.Encoding.ASCII);

            return store.ExecuteDataSet("Form348_Audit_GetAuditInfo", lodid);
        }

        /// <summary>
        /// Saves an A1 (Administrative) audit for a LOD case.
        /// </summary>
        public void SaveAuditA1(int lodId, int validate, bool status, bool orders, bool epts, bool idt, bool pcars, bool eightYear, string other, int lod, int diagnosis, int request, int iOrA, int iOrD, int activites, int determination, int determinationNotCorrect, string comment)
        {
            if (validate == -1)
            {
                store.ExecuteDataSet("Form348_Audit_A1SaveAudit", lodId, -1, status, orders, epts, idt, pcars, eightYear, other, lod, diagnosis, request, iOrA, iOrD, activites, determination, determinationNotCorrect, comment);
            }
            else
            {
                store.ExecuteDataSet("Form348_Audit_A1SaveAudit", lodId, validate, status, orders, epts, idt, pcars, eightYear, other, lod, diagnosis, request, iOrA, iOrD, activites, determination, determinationNotCorrect, comment);
            }
        }

        /// <summary>
        /// Saves a JA (Judge Advocate / Legal) audit for a LOD case.
        /// </summary>
        public void SaveAuditJA(int lodId, int legal, bool standardOfProof, bool deathAndMVA, bool formalPolicy, bool aFI, string other, int proof, int standard, int proofMet, int evidence, int misconduct, int investigation, string comment)
        {
            if (legal == -1)
            {
                store.ExecuteDataSet("Form348_Audit_JASaveAudit", lodId, -1, standardOfProof, deathAndMVA, formalPolicy, aFI, other, proof, standard, proofMet, evidence, misconduct, investigation, comment);
            }
            else
            {
                store.ExecuteDataSet("Form348_Audit_JASaveAudit", lodId, legal, standardOfProof, deathAndMVA, formalPolicy, aFI, other, proof, standard, proofMet, evidence, misconduct, investigation, comment);
            }
        }

        /// <summary>
        /// Saves an SG (Surgeon General / Medical) audit for a LOD case.
        /// </summary>
        public void SaveAuditSG(int lodId, int appropriate, bool dx, bool iSupport, bool epts, bool aggravation, bool principle, string other, int proof, int standard, int proofMet, int evidence, int misconduct, int investigation, string comment)
        {
            //CodeCleanUP
            if (appropriate == -1)
            {
                store.ExecuteDataSet("Form348_Audit_SGSaveAudit", lodId, -1, dx, iSupport, epts, aggravation, principle, other, proof, standard, proofMet, evidence, misconduct, investigation, comment);
            }
            else
            {
                store.ExecuteDataSet("Form348_Audit_SGSaveAudit", lodId, appropriate, dx, iSupport, epts, aggravation, principle, other, proof, standard, proofMet, evidence, misconduct, investigation, comment);
            }
        }
    }
}