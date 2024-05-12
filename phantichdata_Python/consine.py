import numpy as np

# Định nghĩa ma trận người dùng với các đánh giá cho mỗi sản phẩm
data = np.array([
    [2.8, 5.0, 2.0],  # Đánh giá của người dùng Thaibaohaonhien1995@gmail.com
    [4.5, 3.0, 5.0],  # Đánh giá của người dùng coLuan246@gmail.com
    [3.0, 3.0, 0.0]   # Đánh giá của người dùng tht252003@gmail.com
])

# Tính độ dài (norm) của mỗi vectơ người dùng
norms = np.linalg.norm(data, axis=1)
#print(norms)

# Tính tích vô hướng giữa các cặp người dùng
dot_product = np.dot(data, data.T)
#print (dot_product)

# Tính độ tương đồng cosine giữa các người dùng
# Sử dụng np.outer để chia các tổ hợp của norms, tránh chia cho số 0
#print(np.outer(norms, norms))
cosine_similarity = dot_product / np.outer(norms, norms)


# Xuất ra ma trận độ tương đồng cosine


print(cosine_similarity)
