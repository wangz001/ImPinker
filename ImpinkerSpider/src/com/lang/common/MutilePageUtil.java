package com.lang.common;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class MutilePageUtil {

	private volatile static MutilePageUtil mutilePageUtil = null;

	private static Map<String, List<MutilePageModel>> objectMap = new HashMap<String, List<MutilePageModel>>();

	/**
	 * 单利模式，获取实例
	 * 
	 * @return
	 */
	public static MutilePageUtil getInstance() {
		if (mutilePageUtil == null) {
			synchronized (SolrJUtil.class) { // 加锁，解决多线程使用单例的问题
				mutilePageUtil = new MutilePageUtil();
			}
		}
		return mutilePageUtil;
	}

	private MutilePageUtil() {
	}

	public void AddMutilPage(MutilePageModel pageModel) {
		if (objectMap.containsKey(pageModel.getPageKey())) {
			List<MutilePageModel> pages = objectMap.get(pageModel.getPageKey());
			pages.add(pageModel);

			if (pages.size() == pageModel.getOtherPages().size()) {// 所有页面下载完，进行合并
				CombinePages(pages);
				objectMap.remove(pageModel.getPageKey());
			} else {
				objectMap.put(pageModel.getPageKey(), pages);
			}
		} else {
			List<MutilePageModel> list = Arrays.asList(pageModel);
			List<MutilePageModel> arrayList = new ArrayList<MutilePageModel>(
					list);
			objectMap.put(pageModel.getPageKey(), arrayList);
		}
	}

	private void CombinePages(List<MutilePageModel> pages) {

		Collections.sort(pages, new Comparator<MutilePageModel>() {
			@Override
			public int compare(MutilePageModel o1, MutilePageModel o2) {
				try {
					int i1 = Integer.parseInt(o1.getPageindex());
					int i2 = Integer.parseInt(o2.getPageindex());
					return i1 - i2;
				} catch (NumberFormatException e) {
					return o1.getPageindex().compareTo(o2.getPageindex());
				}
			}
		});

		System.out.println("合并。。。。。。。。。。");
	}
}
