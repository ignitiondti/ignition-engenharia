namespace summary.api.Repositorys.Querys
{
    public static class SummarysQuerys
    {
        public const string GET_LATEST_SUMMARY = @"
            SELECT TOP 1
                Id,
                Summary,
                FileName,
                CreatedDate
            FROM
                Summary
            ORDER BY
                CreatedDate DESC";

        public const string GET_SUMMARY_BY_ID = @"
            SELECT
                Id,
                Summary,
                FileName,
                CreatedDate
            FROM
                Summary
            WHERE
                Id = @Id";

        public const string SAVE_SUMMARY = @"
            INSERT INTO Summary
            (
                Summary,
                FileName,
                CreatedDate
            )
            VALUES
            (
                @Summary,
                @FileName,
                @CreatedDate
            )
            SELECT CAST(scope_identity() AS INT)";

    }
}
