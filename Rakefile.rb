# Rakefile.rb
require 'rubygems'      
require 'ftools'

ILMERGE = "ilmerge"
DEVENV = "\"C:\\Program Files\\Microsoft Visual Studio 9.0\\Common7\\IDE\\devenv.exe\""

SOLUTION_ROOT = Dir.pwd
SOLUTION_FILE = "\"#{SOLUTION_ROOT}\\src\\automoq.sln\""

BIN_DIRECTORY = "#{SOLUTION_ROOT}\\src\\AutoMoq\\bin\\Release"

DEPLOY_DIRECTORY = "#{SOLUTION_ROOT}\\deploy"
DEPLOY_FILE =  "#{DEPLOY_DIRECTORY}\\AutoMoq.dll"

task :default=>[:say_hi]

desc "Says hi"
task :say_hi do 
	puts "Hi there. Rake is working."
	puts "Type rake --tasks."
end

desc "Count the files of each type in this solution"
task :file_count do
	report = {}
	Dir['./**/**'].each do |f| 
		report[File.extname(f)].nil? ? report[File.extname(f)] = 1 : report[File.extname(f)] += 1 
	end
	report.each do |type, count|
		puts "#{type}: #{count}"
	end   
end

desc "Cleans the solution"
task :clean do
	sh "#{DEVENV} #{SOLUTION_FILE} /clean"
end

desc "Builds the release version"
task :release_build do
	sh "#{DEVENV} #{SOLUTION_FILE} /build Release"
end

desc "Create the AutoMoq.dll in /deploy"
task :create_deploy do
    Rake::Task['release_build'].execute
	mkdir "#{DEPLOY_DIRECTORY}" unless File.exists?(DEPLOY_DIRECTORY)
	includes = []
	Dir["#{BIN_DIRECTORY}/*.dll"].each do |f| 
		if (File.extname(f) == '.dll')
			includes.push f
		end
	end
	sh "#{ILMERGE} /t:library /out:#{DEPLOY_FILE} #{includes.join(" ")}"
	Rake::Task['clean'].execute
end

desc "Delete the /deploy directory"
task :clean_deploy do
	File.delete "#{DEPLOY_DIRECTORY + "/AutoMoq.dll"}" if File.exists?(DEPLOY_DIRECTORY + "/AutoMoq.dll")
	File.delete "#{DEPLOY_DIRECTORY + "/AutoMoq.pdb"}" if File.exists?(DEPLOY_DIRECTORY + "/AutoMoq.pdb")
	Dir.delete "#{DEPLOY_DIRECTORY}" if File.exists?(DEPLOY_DIRECTORY)
end