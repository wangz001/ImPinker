package com.lang.common;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.lang.fblife.FbLifeXPathCommon;
import com.lang.util.TUtil;

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
		CombineFB(pages);
		System.out.println("合并。。。。。。。。。。");
	}

	private void CombineFB(List<MutilePageModel> pages) {
		MutilePageModel model = pages.get(0);
		String otherContent = "";
		for (MutilePageModel mutilemodel : pages) {
			if (pages.indexOf(mutilemodel) == 0) {
				continue;
			}
			otherContent += FbLifeXPathCommon.getContentString(mutilemodel
					.getPage());
		}
		String titleString = FbLifeXPathCommon.getTitleString(model.getPage());
		if (titleString != null && titleString.length() > 0) {
			String url = FbLifeXPathCommon.getUrl(model.getPage());
			String firstImg = FbLifeXPathCommon.getFirstImg(model.getPage());
			String keyWord = FbLifeXPathCommon
					.getKeyWordString(model.getPage());
			String description = FbLifeXPathCommon.getDescription(model
					.getPage());
			String content = FbLifeXPathCommon
					.getContentString(model.getPage()) + otherContent;
			String publishTime = FbLifeXPathCommon.getPublishTime(model
					.getPage());

			Article article = new Article();
			article.setTitle(titleString);
			article.setKeyWord(keyWord);
			if (description.length() > 200) {
				article.setDescription(description.substring(0, 198));
			} else {
				article.setDescription(description);
			}
			article.setUrlString(url);
			article.setCompany(CompanyEnum.Fblife.getName());
			article.setCoverImage(firstImg);
			article.setContent(content);
			String timeString = "" != publishTime ? publishTime : TUtil
					.getCurrentTime();

			article.setCreateTime(timeString);
			// 根据url获取id。如果id>0，表示存在，则重新做索引。如果==0，则表示不存在
			ArticleDao articleDao = new ArticleDao();
			long id = articleDao.GetIdByUrl(article.urlString);
			if (id > 0) {
				article.setId(id);
			} else {
				id = articleDao.Add(article);
				article.setId(id);
			}
			String timeStr = TUtil.strToUTCTime(article.getCreateTime());
			article.setCreateTime(timeStr); // 转成utc时间格式
			// 添加索引
			SolrJUtil solrJUtil = SolrJUtil.getInstance();
			solrJUtil.AddDocs(article);
		}

	}
}
