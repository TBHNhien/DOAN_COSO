import pandas as pd
from sklearn.neighbors import NearestNeighbors
import numpy as np

# Đọc dữ liệu từ file Excel
file_path = 'data_train2.xls'  # Thay thế đường dẫn file thực tế của bạn
data = pd.read_excel(file_path)

# Tiền xử lý dữ liệu để tạo ra ma trận đánh giá
# Pivot table để người dùng là hàng và sản phẩm là cột, giá trị là đánh giá đã tính trung bình sẵn
ratings_matrix = data.pivot_table(index='UserName', columns='ProductId', values='AvgRate').fillna(0)
print(ratings_matrix)


# Áp dụng thuật toán NearestNeighbors
model_knn = NearestNeighbors(metric='cosine', algorithm='brute')
model_knn.fit(ratings_matrix)

# Giả sử chúng ta muốn đề xuất cho người dùng đầu tiên trong ma trận
current_user = ratings_matrix.iloc[0].values.reshape(1, -1)

# Tìm k người dùng gần nhất - ở đây k=2
distances, indices = model_knn.kneighbors(current_user, n_neighbors=3)

# Khuyến nghị sản phẩm dựa trên đánh giá của người dùng gần nhất
similar_users = ratings_matrix.index[indices.flatten()[1:]].tolist()  # Loại bỏ người dùng hiện tại
recommendations = ratings_matrix.loc[similar_users].mean(axis=0).nlargest(6).index
print("data recomment")
print(len(recommendations))

#Kiểm tra độ dài của danh sách recommendations
if len(recommendations) >= 4:
    print(f"Sản phẩm được khuyến nghị cho người dùng {ratings_matrix.index[0]} là: Product ID {recommendations[0]}, {recommendations[1]}, {recommendations[2]}, {recommendations[3]},{recommendations[4]},{recommendations[5]}")
else:
    print(f"Sản phẩm được khuyến nghị cho người dùng {ratings_matrix.index[0]} là: {', '.join(map(str, recommendations))}")


# Xuất ra sản phẩm được đề xuất
#print(f"Sản phẩm được khuyến nghị cho người dùng {ratings_matrix.index[0]} là: Product ID {recommendations[0]}")
