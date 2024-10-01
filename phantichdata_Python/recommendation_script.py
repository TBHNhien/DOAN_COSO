import json
import pandas as pd
from sklearn.neighbors import NearestNeighbors

# Đọc dữ liệu JSON từ đối số dòng lệnh
import sys
input_json = sys.argv[1]

user_reviews = json.loads(input_json)

# Đọc dữ liệu JSON từ đối số dòng lệnh
#input_json = sys.stdin.readline().strip()
#user_reviews = json.loads(input_json)

# Chuỗi JSON trực tiếp
input_json = '''
[
    { "ProductId": 4, "Rating": 3 },
    { "ProductId": 8, "Rating": 5 }
]
'''

# Phân tích chuỗi JSON
#user_reviews = json.loads(input_json)

# Chuyển đổi JSON thành DataFrame
user_data = pd.DataFrame(user_reviews)



# Đọc dữ liệu đánh giá từ file hoặc cơ sở dữ liệu
file_path = r'C:\Users\Admin\Desktop\DOAN_CS\DOAN_COSO\phantichdata_Python\data_train2.xls'
all_data = pd.read_excel(file_path)

# Tạo ma trận đánh giá
ratings_matrix = all_data.pivot_table(index='UserName', columns='ProductId', values='AvgRate').fillna(0)

# Áp dụng thuật toán NearestNeighbors
model_knn = NearestNeighbors(metric='cosine', algorithm='brute')
model_knn.fit(ratings_matrix)

# Tìm k người dùng gần nhất dựa trên dữ liệu đầu vào của người dùng hiện tại
current_user_ratings = pd.DataFrame([user_data.set_index('ProductId')['Rating']], columns=ratings_matrix.columns).fillna(0)
distances, indices = model_knn.kneighbors(current_user_ratings, n_neighbors=3)

# Đề xuất sản phẩm dựa trên đánh giá của người dùng gần nhất
similar_users = ratings_matrix.index[indices.flatten()[1:]].tolist()
recommendations = ratings_matrix.loc[similar_users].mean(axis=0).nlargest(6).index

print(f"{recommendations[0]},{recommendations[1]},{recommendations[2]},{recommendations[3]},{recommendations[4]},{recommendations[5]}")
