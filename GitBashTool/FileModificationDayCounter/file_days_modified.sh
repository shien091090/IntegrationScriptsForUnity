#!/bin/bash

# 定义一个临时文件
temp_file=$(mktemp)

# 只遍历 Assets/Scripts/ 路径下的 .cs 文件
for file in $(git ls-files | grep 'Assets/Scripts/.*\.cs$'); do
    # 计算有多少天进行了修改
    days_modified=$(git log --pretty=format:'%cd' --date=short -- $file | uniq | wc -l)
    # 输出到临时文件，格式为：天数 文件名
    echo "$days_modified $file" >> $temp_file
done

# 使用sort命令按数字进行逆序排序，并输出
sort -rn -k1,1 $temp_file | while read -r line; do
    echo "${line#* } : ${line%% *}"
done

# 删除临时文件
rm -f $temp_file