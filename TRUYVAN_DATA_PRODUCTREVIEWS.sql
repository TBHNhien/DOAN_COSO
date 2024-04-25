SELECT * FROM ProductReviews
GO
SELECT * FROM AspNetUsers
GO


--DÙNG JOIN 
--SELECT PR.ReviewId, PR.ProductId, AU.UserName, PR.Rating, PR.ReviewText, PR.ReviewDate
--FROM ProductReviews PR
--JOIN AspNetUsers AU ON PR.UserId = AU.Id

--DÙNG WHERE
SELECT PR.ReviewId, PR.ProductId, AU.UserName, PR.Rating, PR.ReviewText, PR.ReviewDate
FROM ProductReviews PR, AspNetUsers AU
WHERE PR.UserId = AU.Id

