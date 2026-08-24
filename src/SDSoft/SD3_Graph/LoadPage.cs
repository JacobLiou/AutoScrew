using System.Data;

namespace SD3_Graph
{
	internal class LoadPage
	{
		public DataTable dtCurrent;

		public int currentPage = 0;

		private int pageSize;

		private int recordCount = 0;

		private int pageCount = 0;

		public LoadPage(int recordCount, int pageSize)
		{
			this.pageSize = pageSize;
			this.recordCount = recordCount;
			pageCount = recordCount / pageSize;
			if (recordCount % pageSize > 0)
			{
				pageCount++;
			}
			currentPage = 1;
		}

		public void ChangePage(DataTable dt, int currentPage)
		{
			this.currentPage = currentPage;
			recordCount = dt.Rows.Count;
			pageCount = recordCount / pageSize;
			if (dt.Rows.Count % pageSize > 0)
			{
				pageCount++;
			}
			if (this.currentPage < 1)
			{
				this.currentPage = 1;
			}
			if (this.currentPage > pageCount)
			{
				this.currentPage = pageCount;
			}
			dtCurrent = dt.Clone();
			int beginRecord = pageSize * (this.currentPage - 1);
			if (this.currentPage == 1)
			{
				beginRecord = 0;
			}
			int endRecord = pageSize * this.currentPage;
			if (this.currentPage == pageCount)
			{
				endRecord = recordCount;
			}
			if (recordCount == 0)
			{
				return;
			}
			for (int i = beginRecord; i < endRecord; i++)
			{
				if (i < dt.Rows.Count)
				{
					dtCurrent.ImportRow(dt.Rows[i]);
				}
			}
		}
	}
}
