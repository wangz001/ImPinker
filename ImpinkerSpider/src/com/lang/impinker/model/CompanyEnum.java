package com.lang.impinker.model;

public enum CompanyEnum {
	Fblife("越野e族", 1), Autohome("汽车之家", 2), Yiche("易车", 3), Xcar("爱卡汽车", 4), PCauto(
			"太平洋汽车", 5);
	// 成员变量
	private String name;
	private int index;

	// 构造方法
	private CompanyEnum(String name, int index) {
		this.name = name;
		this.index = index;
	}

	// 普通方法
	public static String getName(int index) {
		for (CompanyEnum c : CompanyEnum.values()) {
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
