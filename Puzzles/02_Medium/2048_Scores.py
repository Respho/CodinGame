import sys
import math

nums = []
for i in range(4):
    for j in input().split():
        cnt = int(j)
        nums.append(cnt)
fours = int(input())

totalScore = 0
for n in nums:
    if n > 0:
        totalScore += (math.log(n, 2.0) - 1.0) * n
totalScore -= fours * 4

turns = (sum(nums) - fours * 2) / 2 - 2

print(int(totalScore))
print(int(turns))
