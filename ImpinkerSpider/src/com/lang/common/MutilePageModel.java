package com.lang.common;

import java.util.List;

import us.codecraft.webmagic.Page;

public class MutilePageModel {

	private String pageKey;

	private String pageindex;

	private List<String> allPages;

	private Page page;

	public String getPageKey() {
		return pageKey;
	}

	public void setPageKey(String pageKey) {
		this.pageKey = pageKey;
	}

	public String getPageindex() {
		return pageindex;
	}

	public void setPageindex(String pageindex) {
		this.pageindex = pageindex;
	}

	public List<String> getOtherPages() {
		return allPages;
	}

	public void setOtherPages(List<String> otherPages) {
		this.allPages = otherPages;
	}

	public Page getPage() {
		return page;
	}

	public void setPage(Page page) {
		this.page = page;
	}

}
