import pandas as pd
from collections import Counter
from nltk.corpus import stopwords
import string


# Bước 1: Đọc dữ liệu từ file Excel
def read_reviews(file_path):
    try:
        print("Đang đọc file:")
        # Sử dụng thư viện Pandas để đọc file
        data = pd.read_excel(file_path)
        return data
    except Exception as e:
        print("Có lỗi xảy ra khi đọc file:", e)
        return None


# Danh sách stopwords cho tiếng Việt, bạn có thể mở rộng hoặc chỉnh sửa nó
vietnamese_stopwords = [
    'và', 'của', 'bị', 'là', 'thì', 'được',
    # Thêm vào các từ tiếng Việt thường gặp khác mà bạn muốn loại bỏ
]
# Bước 2: Tiền xử lý dữ liệu
def preprocess_text(text):
    # Loại bỏ dấu câu
    text = text.translate(str.maketrans('', '', string.punctuation))
    # Chuyển đổi thành chữ thường
    text = text.lower()
    # Loại bỏ các từ dừng (stopwords)
    #stop_words = set(stopwords.words('english'))  # Sử dụng ngôn ngữ phù hợp
    words = text.split()
    #words = [word for word in words if word not in stop_words]
    words = [word for word in words if word not in vietnamese_stopwords]
    return words

# Bước 3: Phân tích tần suất từ
# def analyze_frequency(data):
#     all_words = []
#     for review in data['ReviewText']:
#         all_words.extend(preprocess_text(review))
#     word_counts = Counter(all_words)
#     return word_counts

def analyze_frequency(data):
    all_word_counts = Counter()

    # Nhóm dữ liệu theo 'ProductId'
    grouped_data = data.groupby('ProductId')

    # Lặp qua từng nhóm dữ liệu
    for product_id, group_data in grouped_data:
        # Tạo danh sách từ cho từng nhóm dữ liệu
        group_words = []
        for review in group_data['ReviewText']:
            group_words.extend(preprocess_text(review))

        # Tính toán tần suất từ cho từng nhóm
        group_word_counts = Counter(group_words)

        # Kết hợp tần suất từ của từng nhóm vào tần suất từ toàn bộ dữ liệu
        all_word_counts += group_word_counts

    return all_word_counts


# Bước 4: Tính toán điểm đánh giá trung bình
def calculate_average_ratings(data):
    average_ratings = data.groupby('ProductId')['Rating'].mean()
    return average_ratings

# Hàm chính để chạy chương trình
def main(file_path):
    # Đọc dữ liệu từ file
    reviews_data = read_reviews(file_path)
    if reviews_data is not None:
        # Phân tích tần suất từ
        word_freq = analyze_frequency(reviews_data)
        print("Tần suất từ trong đánh giá:", word_freq)

        # Tính điểm đánh giá trung bình
        avg_ratings = calculate_average_ratings(reviews_data)
        print("Điểm đánh giá trung bình:", avg_ratings)

if __name__ == "__main__":
    # Đường dẫn tới file Excel
    excel_file_path = 'data_reviews.xls'
    main(excel_file_path)
