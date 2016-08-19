package com.lang.common;

public enum ArticleTypeEnum {

	XinWen("新闻", 1), LvXing("旅行", 2), YueYe("越野", 3), ZiJiaYou("自驾游", 4), PingCe(
			"评测", 5), ChuanYue("穿越", 6), GaiZhuang("改装", 7), WenHua("文化", 8);
	// 成员变量
	private String name;
	private int index;

	// 构造方法
	private ArticleTypeEnum(String name, int index) {
		this.name = name;
		this.index = index;
	}

	// 普通方法
	public static String getName(int index) {
		for (ArticleTypeEnum c : ArticleTypeEnum.values()) {
			if (c.getIndex() == index) {
				return c.name;
			}
		}
		return null;
	}

	// get set 方法
	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public int getIndex() {
		return index;
	}

	public void setIndex(int index) {
		this.index = index;
	}
}
