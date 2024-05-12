import numpy as np

# Ma trận độ tương đồng cosine
cosine_similarity = np.array([
    [1.0, 0.84106229, 0.90869846],
    [0.84106229, 1.0, 0.72002304],
    [0.90869846, 0.72002304, 1.0]
])

# Ma trận đánh giá của người dùng
ratings_data = np.array([
    [2.8, 5.0, 2.0],  # Đánh giá của người dùng 1
    [4.5, 3.0, 5.0],  # Đánh giá của người dùng 2
    [3.0, 3.0, 0.0]   # Đánh giá của người dùng 3
])

# Chọn người dùng đầu tiên để đề xuất
user_index = 0

# Lấy độ tương đồng của người dùng đã chọn với tất cả người dùng khác
similarity_scores = cosine_similarity[user_index]
print(similarity_scores)

# Tính điểm đánh giá trung bình có trọng số
weighted_ratings = np.dot(similarity_scores, ratings_data)
print(weighted_ratings)

# Tính tổng độ tương đồng để chuẩn hóa (không tính độ tương đồng của chính nó)
sum_similarity_scores = np.sum(similarity_scores) - 1  # Trừ đi 1 để không tính độ tương đồng của chính nó
# print(sum_similarity_scores)

# Tính điểm trung bình có trọng số
weighted_average = weighted_ratings / sum_similarity_scores
print(weighted_average)

# Tìm sản phẩm được khuyến nghị (chỉ số sản phẩm có điểm cao nhất)
recommended_product_index = np.argmax(weighted_average)
print(recommended_product_index)

recommended_product_id = recommended_product_index + 4  # +4 để điều chỉnh cho ProductId thực tế bắt đầu từ 4

recommended_product_id, weighted_average
# print(recommended_product_id)
# print(weighted_average)