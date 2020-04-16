using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Model.TempDataObject
{
	public class AirdragComponentData : IAirdrag
	{
		#region Members
		
		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public string CertificationNumber { get; set; }
		public DateTime? Date { get; set; }
		public bool UseMeasuredValues { get; set; }
		public SquareMeter CdxA_0 { get; set; }
		public SquareMeter TransferredCdxA { get; set; }
		public SquareMeter DeclaredCdxA { get; set; }
		public string AppVersion { get; set; }

		#endregion

		public AirdragComponentData(IAirdragViewModel airdrag)
		{
			SetValues(airdrag);
		}
		
		public void UpdateCurrentValues(IAirdragViewModel airdrag)
		{
			SetValues(airdrag);
		}

		private void SetValues(IAirdragViewModel airdrag)
		{
			Model = airdrag.Model;
			Manufacturer = airdrag.Manufacturer;
			CertificationNumber = airdrag.CertificationNumber;
			Date = airdrag.Date;
			AppVersion = airdrag.AppVersion;
			DeclaredCdxA = airdrag.DeclaredCdxA;
			CdxA_0 = airdrag.CdxA_0;
			TransferredCdxA = airdrag.TransferredCdxA;
		}
	}
}
